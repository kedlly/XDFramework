#!/bin/bash


# 帮助函数放在最开始，方便调用
function usage() {
    echo "Usage: $0 -p <ios or android> [-s 1 2 3 ...]"
    echo
    echo "Example:"
    echo "$0 -p ios -s 123"
    echo "if -s specified. 1: unity export, 2: <anroid build> or <ios archive>, 3: ios xcarchive export to ipa"
    exit 1
}

function echoerr() {
    echo "$@" 1>&2;
}

SHELL_PATH=$( cd "$(dirname "$0")"; pwd)

#当前配置的包名称
PROJECT_PACKAGE=`echo $SHELL_PATH | awk -F "/" '{print $NF}'`


STEPS=123

############### 参数分析 ###############
# option_string以冒号开头表示屏蔽脚本的系统提示错误，自己处理错误提示。
# 后面接合法的单字母选项，选项后若有冒号，则表示该选项必须接具体的参数
while getopts ":p:b:l:s:" OPTION; do
    case "${OPTION}" in
        p)
			BUILD_TARGET=${OPTARG}
            ;;
        b)
 			PROJECT_PACKAGE=${OPTARG}
            ;;
		s)
			STEPS=${OPTARG}
            ;;
        *)
            usage
            ;;
    esac
done



if [ -z "$BUILD_TARGET" ]; then
	 echoerr "You must specify BUILD_TARGET with -p option"
	 exit 1
fi

if [ -z "$PROJECT_PACKAGE" ]; then
	echoerr "You must specify PROJECT_PACKAGE with -b option"
	exit 1
fi

if [ -z "$STEPS" ]; then
	echoerr "You must specify BUILD STEP with -s option"
	exit 1
fi	


################################### 项目配置 START ##################################

echo ${UnityApplication="/Applications/Unity/Unity.app/Contents/MacOS/Unity"} > /dev/null

echo ${PROJECT_PATH=$( cd "$SHELL_PATH"/../../../; pwd)} > /dev/null

echo ${GRADLE_PATH="~/gradle-3.5/bin"} > /dev/null

echo ${EXPORT_PATH=$PROJECT_PATH/ProjectBuild/Packages/$PROJECT_PACKAGE/$BUILD_TARGET} > /dev/null

echo ${LOGFILE_PATH=$EXPORT_PATH/../$BUILD_TARGET.Build.log} > /dev/null


#define ENV Vars
UnityApplication="/Applications/Unity5/Unity.app/Contents/MacOS/Unity"


LOGFILE_PATH=/dev/stdout
set -x
################################### 项目配置 END ##################################

PATH=$PATH:$GRADLE_PATH
export PATH


APP_PATH=$EXPORT_PATH/../Apps/$BUILD_TARGET

function ClearExportAppPath()
{
	echo clean $APP_PATH
	if [ ! -d $APP_PATH ]; then
		mkdir -p $APP_PATH
	else
		rm -rf $APP_PATH/*
	fi
}

function ExportUnityProject()
{	
	if [ -d $EXPORT_PATH ]; then
		rm -rf $EXPORT_PATH/*
	else
		mkdir -p $EXPORT_PATH
	fi
	
	echo "Start Export $BUILD_TARGET project from Unity Editor."
	echo "Please wait ..."
	
	$UnityApplication \
		-batchmode \
		-projectPath "$PROJECT_PATH" \
		-nographics \
		-quit \
		-logFile $LOGFILE_PATH \
		-executeMethod CommandLineFunction.Build \
		    clf.outDir=$EXPORT_PATH \
		    clf.cfgPackage=$PROJECT_PACKAGE \
		    clf.group=$BUILD_TARGET \
			clf.target=$BUILD_TARGET
	
	
	if [ "$?" != "0" ]; then
		echo Unity Application Can not Export Target Project, Fix what happened and try again.
		exit 1
	else
		echo "Unity Export $BUILD_TARGET project Succeeded"
	fi
}

function tryRun()
{
	local PLAT=$1
	local SCRIPT=$2
	local TARGET="$SHELL_PATH/$PLAT/$SCRIPT"
	if [ -f "$TARGET" ]; then
		if [ ! -x "$TARGET" ]; then
			echo "$TARGET" cannot run, try use 'chmod +x <file>' fix it
			chmod +x $TARGET
		fi	
		$TARGET
		if [ "$?" -ne "0" ]; then
			echoerr "PreBuild Error, Check Script ."
			exit 1
		fi
	fi
}

function BuildAndorid()
{
	ClearExportAppPath
	
	cd $EXPORT_PATH
	for item in `ls`
	do
		if [ -d $item ] ;then
			cd $item
			break
		fi
	done
	
	echo output project : `pwd`
	
	tryRun Android PreBuild.sh
	
	sed -i ".bak" 's/gradle:2.1.0/gradle:2.2.0/' build.gradle
	gradle --offline build
	cp ./build/outputs/apk/*.apk $APP_PATH
	tryRun Android PostBuild.sh
}


function BuildIOS()
{
	if [ "$1" == "2" ]; then
		ClearExportAppPath
	fi
	cd $EXPORT_PATH
	echo output project : `pwd`
			
	tryRun iOS PreBuild.sh

	# 对于ssh连上mac的终端，签名的时候会提示：User interaction is not allowed.
    # 所以要先对证书解锁。直接在终端执行可以不必要
    UNLOCK=`security show-keychain-info ~/Library/Keychains/login.keychain 2>&1|grep no-timeout`
    if [ -z "$UNLOCK" ]; then
        # -p 后面跟的是密码，各机器可能不一样，要修改
        security unlock-keychain -p 123u123u ~/Library/Keychains/login.keychain
        # 修改unlock-keychain过期时间, 最好大于一次打包时间 
        security set-keychain-settings -t 3600 -l ~/Library/Keychains/login.keychain
    fi
	CODE_SIGN_IDENTITY="iPhone Distribution: Shanghai Jiang You Information Technology Company Limited"
	PROVISIONING_PROFILE="jjsylite"
	METHOD="enterprise"
	
	XCODE_PROJECT_FILE="./Unity-iPhone.xcodeproj"
	BUILD_CFG="Release"
	SCHEME="Unity-iPhone"

	$SHELL_PATH/iosbuild.sh $XCODE_PROJECT_FILE \
		-s $SCHEME \
		-c $BUILD_CFG \
		-o "$APP_PATH" \
		-i "${CODE_SIGN_IDENTITY}" \
		-v "${PROVISIONING_PROFILE}" \
		-m "$METHOD"\
		-t $1

    # 如果返回1，那么说明编译出错了
    if [ $? -ne 0 ]; then
        echoerr "iOS compiled with error, please check it."
        exit 1
    fi

	tryRun iOS PostBuild.sh
}

function Build()
{
	local ALL_STEPS="$STEPS"
	for ((i=1;i<=${#ALL_STEPS};i++)) do
		local CURRENT_STEP=`echo $ALL_STEPS | cut -c$i`
		if [ "$CURRENT_STEP" == '1' ]; then
			ExportUnityProject
		elif [ "$CURRENT_STEP" == '2' ]; then
			if [ "$BUILD_TARGET" = "android" ]; then
				BuildAndorid
			elif [ "$BUILD_TARGET" = "ios" ]; then
				BuildIOS 1
			fi
		elif [ "$CURRENT_STEP" == '3' ]; then
			if [ "$BUILD_TARGET" = "ios" ]; then
				BuildIOS 2
			fi
		fi
	done
}

Build

#if [ $? -eq 0 ] ; then
#	echo Build Done ...
#else
#	echo Some Promble need to resolve
#fi
