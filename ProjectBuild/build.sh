#!/bin/bash


# 帮助函数放在最开始，方便调用
function usage() {
    echo "Usage: $0 -p <ios, android or all> -b <setting package>"
    echo
    echo "Example:"
    echo "$0 -p android -b xxxx"
    echo
    exit 1
}

function echoerr() {
    echo "$@" 1>&2;
}

ALL="all"

BUILD_TARGET=$ALL

############### 参数分析 ###############
# option_string以冒号开头表示屏蔽脚本的系统提示错误，自己处理错误提示。
# 后面接合法的单字母选项，选项后若有冒号，则表示该选项必须接具体的参数
while getopts ":p:b:" OPTION; do
    case "${OPTION}" in
        p)
			BUILD_TARGET=${OPTARG}
            ;;
        b)
			PROJECT_PACKAGE=${OPTARG}
            ;;
        *)
            usage
            ;;
    esac
done



if [ -z "$BUILD_TARGET" ]; then
	echoerr "You must set BUILD_TARGET with -p option"
	exit 1
fi

if [ -z "$PROJECT_PACKAGE" ]; then
	echoerr "You must set PROJECT_PACKAGE with -b option"
	exit 1
fi


################################### 项目配置 START ##################################

#define ENV Vars

echo ${GRADLE_PATH="/Users/Shared/Unity/PackageDependence/gradle-3.5/bin"} > /dev/null

UnityApplication="/Applications/Unity5/Unity.app/Contents/MacOS/Unity"


################################### 项目配置 END ##################################

PATH=$PATH:$GRADLE_PATH
export PATH
export UnityApplication
export GRADLE_PATH


SHELL_PATH=$( cd "$(dirname "$0")"; pwd)

PROCESS_BACKGROUND1=
PROCESS_BACKGROUND2=

if [ "$BUILD_TARGET" == "$ALL" ]; then
	($SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p ios -s 1)
	if [ $? -eq 0 ]; then
		(  $SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p ios -s 23) &
		PROCESS_BACKGROUND1=$!
	else
		echo "Unity Exprot ios Project Failed in Build all Packages ."
		exit 1
	fi
	( $SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p android -s 1)
	if [ $? -eq 0 ]; then
		( $SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p android -s 2) &
		PROCESS_BACKGROUND2=$!
	else
		echo "Unity Exprot android Project Failed in Build all Packages ."
		exit 1
	fi
	wait #PROCESS_BACKGROUND1
else
	$SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p $BUILD_TARGET
	if [ $? -ne 0 ]; then
		echo "Build $BUILD_TARGET failed."
		exit 1
	fi
fi

echo "Build $BUILD_TARGET done"



