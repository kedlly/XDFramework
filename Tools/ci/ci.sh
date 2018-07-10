#!/bin/bash
#
#set -x

# 帮助函数放在最开始，方便调用
function usage() {
    echo "Usage: $0 -p <ios, android or all, default is all> -v <version string> -b <branch name, default is master>"
    echo
    exit 1
}

function echoerr() {
    echo "$@" 1>&2;
}

BRANCH="master"
ALL="all"
BUILD_TARGET=$ALL

############### 参数分析 ###############
# option_string以冒号开头表示屏蔽脚本的系统提示错误，自己处理错误提示。
# 后面接合法的单字母选项，选项后若有冒号，则表示该选项必须接具体的参数
while getopts ":p:v:b:" OPTION; do
    case "${OPTION}" in
        p)
			BUILD_TARGET=${OPTARG}
            ;;
		v)
			BUILD_VERIOSN=${OPTARG}
			;;
        b)
			BRANCH=${OPTARG}
            ;;
        *)
            usage
            ;;
    esac
done


if [ -z "$BUILD_VERIOSN" ]; then
	echoerr "You must set BUILD_VERIOSN with -v option"
	exit 1
fi


SHELL_PATH=$( cd "$(dirname "$0")"; pwd)


# 项目打包后生成的目标文件存放在ci.sh 当前目录下的 build 目录下
# 如果打包成功
# ios 包存放在 build/iOS 下
# android 包放置 build/Android 下
# Windows 包放在 build/Windows 下
# MacOS 包放在 build/MacOS 下 
# 否则该目录为空

CI_TARGET_PATH=$SHELL_PATH/../../

if [ ! -d ${CI_TARGET_PATH} ]; then
	mkdir -p ${CI_TARGET_PATH}
fi

cd $SHELL_PATH/../../

#git reset --hard

#git clean -fdx

#git checkout $BRANCH

#if [ "$?" -ne "0" ]; then
#	echoerr "there is no branch named $BRANCH in repo."
#fi

#git pull

set -x

PROJECT_PACKAGE="com.joyyou.eaststudio"

cd ${SHELL_PATH}/../../game/Client/ProjectBuild

APP_OUTPUT_PATH=$PWD/Packages/$PROJECT_PACKAGE/Apps

FILENAME_PREFIX="t_"
FTP_SERVER="42.62.51.126"
FTP_PORT="21"
FTP_USER="ms"
FTP_PASSWORD="Zya0eMHcYNfp"

BOOL_MOVE_TARGET_TO_CI_BUILD_PATH=true

function uploadApk()
{
	local APKFILE=`find $APP_OUTPUT_PATH/android -name "*release.apk"`
	if [ -z "$APKFILE" ]; then
		echo "Cannot find android apk file from the package output path : $APP_OUTPUT_PATH/android. maybe build failed."
		echo "Check Build Environment and build shell script ."
		return 1
	fi
	if $BOOL_MOVE_TARGET_TO_CI_BUILD_PATH; then
		if [ ! -d ${CI_TARGET_PATH} ]; then
			mkdir -p ${CI_TARGET_PATH}
		else
			rm $CI_TARGET_PATH/*.apk
		fi
		mv $APKFILE $CI_TARGET_PATH/${FILENAME_PREFIX}${BUILD_VERIOSN}.apk
	else
		echo "Uploading ... $APKFILE";
		lftp -u $FTP_USER,$FTP_PASSWORD -p $FTP_PORT $FTP_SERVER << EOF
			put $APKFILE -o ${FILENAME_PREFIX}${BUILD_VERIOSN}.apk
			bye
EOF
		if [ $? -ne 0  ]; then
			echo "Upload apk file failed, ensure which file you selected to upload is exist."
			return 2
		fi
		echo "upload apk succeed"
	fi
	return 0
}

function uploadIpa()
{
	local IPAFILE=`find $APP_OUTPUT_PATH/ios -name "*.ipa"`
	if [ -z "$IPAFILE" ]; then
		echo "Cannot find ios ipa file from the package output path : $APP_OUTPUT_PATH/ios. maybe build failed."
		echo "Check Build Environment and build shell script ."
		return 1
	fi
	if $BOOL_MOVE_TARGET_TO_CI_BUILD_PATH; then
		if [ ! -d ${CI_TARGET_PATH} ]; then
			mkdir -p ${CI_TARGET_PATH}
		else
			rm $CI_TARGET_PATH/*.ipa
		fi
		mv $IPAFILE $CI_TARGET_PATH/${FILENAME_PREFIX}${BUILD_VERIOSN}.ipa
	else
		echo "Uploading ... $IPAFILE";
		lftp -u $FTP_USER,$FTP_PASSWORD -p $FTP_PORT $FTP_SERVER << EOF
		put $IPAFILE -o ${FILENAME_PREFIX}${BUILD_VERIOSN}.ipa
		bye
EOF
		if [ $? -ne 0  ]; then
			echo "Upload Ipa file failed, ensure which file you selected to upload is exist."
			return 2
		fi
		echo "upload ipa succeed"
	fi
	return 0
}


./build.sh -b $PROJECT_PACKAGE -p $BUILD_TARGET  2>&1 | tee ${SHELL_PATH}/ci.log

if [ ${PIPESTATUS[0]} -eq 0  ]; then
	if [ "$BUILD_TARGET" == "$ALL" ] ;then
		uploadApk
		if [ $? -ne 0 ] ; then
			exit 1
		fi
		uploadIpa
		if [ $? -ne 0 ] ; then
			exit 1
		fi
	elif [ "$BUILD_TARGET" == "android" ] ;then
		uploadApk
		if [ $? -ne 0 ] ; then
			exit 1
		fi
	elif [ "$BUILD_TARGET" == "ios" ] ;then
		uploadIpa
		if [ $? -ne 0 ] ; then
			exit 1
		fi
	fi
    echo "Upload process done"
else
    echo "Package failed, nothing upload."
fi


