#!/bin/bash


# 帮助函数放在最开始，方便调用
function usage() {
    echo "Usage: $0  -b <setting package>"
    exit 1
}

function echoerr() {
    echo "$@" 1>&2;
}


############### 参数分析 ###############
# option_string以冒号开头表示屏蔽脚本的系统提示错误，自己处理错误提示。
# 后面接合法的单字母选项，选项后若有冒号，则表示该选项必须接具体的参数
while getopts ":b:l:" OPTION; do
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

if [ -z "$PROJECT_PACKAGE" ]; then
	echoerr "You must specify PROJECT_PACKAGE with -b option"
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

($SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p ios -s 1)
($SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p ios -s 23) &
($SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p android -s 1)
($SHELL_PATH/Settings/$PROJECT_PACKAGE/Package.sh -p android -s 2) &

wait

echo "Build All platform done"

#
#

