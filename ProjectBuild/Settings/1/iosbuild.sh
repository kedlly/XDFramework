#/bin/bash


function usage()
{
	echo "useage: "
	echo "fast jailbreak package : $0 </path/to/project/file> -b <buildtarget> -c <build config> -j -i <code sign identifity> -v <provisioning profile> -o </output/ipa/path> -t <step option: 1 build, 2 compress or export ipa>" 
    echo "normal package :"
    echo "Example:"
    echo "$0 </path/to/project/file> -s <scheme> -c <build config> -i <code sign identifity> -v <provisioning profile> -m <method> -t <step option: 1 archive, 2 export ipa>"
    echo "note:"
	echo "<build config> like Debug, Relase .etc ."
    echo "<buildtarget> and <scheme> is in you XCode Settings"
	echo "<method> is one of app-store, ad-hoc, package, enterprise, development, and developer-id"
	echo "if -d specified, debug mode enabled."
	echo "-t is optional"
	exit 1
}

SHELL_PATH=$( cd "$(dirname "$0")"; pwd)

PROJECT_FILE=$1

shift


TRUE="true"
FALSE="false"
JB_PACK=$FALSE
DEBUG_MODE=$TRUE
STEPS="12"

############### 参数分析 ###############
# option_string以冒号开头表示屏蔽脚本的系统提示错误，自己处理错误提示。
# 后面接合法的单字母选项，选项后若有冒号，则表示该选项必须接具体的参数
while getopts ":b:c:i:v:jds:m:o:t:" OPTION; do
    case "${OPTION}" in
        b)
			BUILD_TARGET=${OPTARG}
            ;;
		c)
			BUILD_CFG=${OPTARG}
            ;;
		i)
			CODE_SIGN_IDENTITY=${OPTARG}
			;;
		v)
			PROVISIONING_PROFILE=${OPTARG}
            ;;
		j)
			JB_PACK=$TRUE
            ;;
		s)
			ARCHIVE_SCHEME=${OPTARG}
            ;;
		o)
			IPA_TARGET_PATH=${OPTARG}
            ;;
		m)
			ARCHIVE_METHOD=${OPTARG}
            ;;
		t)
			STEPS=${OPTARG}
            ;;
		d)
			DEBUG_MODE=$TRUE
			set -x
            ;;	
        *)
            usage
            ;;
    esac
done


function echoerr() {
    echo "$@" 1>&2;
}

if [ ! -d "$PROJECT_FILE" ]; then
	echoerr "You must specify PROJECT_FILE with the first argument"
fi

if [ $JB_PACK = $TRUE ]; then
	if [ -z ${BUILD_TARGET} ] ; then
		echoerr "You must specify BUILD_TARGET with -b option"
	fi
else
	if [ -z "$ARCHIVE_METHOD" ]; then
		echoerr "You must specify ARCHIVE_METHOD with -m option"
	fi
fi

if [ -z ${BUILD_CFG} ] ; then
	echoerr "You must specify BUILD_CFG with -c option"
fi

if [ -z "${CODE_SIGN_IDENTITY}" ] ; then
	echoerr "You must specify CODE_SIGN_IDENTITY with -i option"
fi

if [ -z "${PROVISIONING_PROFILE}" ] ; then
	echoerr "You must specify PROVISIONING_PROFILE with -v option"
fi

if [ -z "${ARCHIVE_SCHEME}" ] ; then
	echoerr "You must specify ARCHIVE_SCHEME with -s option"
fi

if [ -z "${IPA_TARGET_PATH}" ] ; then
	echoerr "You must specify IPA_TARGET_PATH with -o option"
fi

if [ ! -d "${IPA_TARGET_PATH}" ] ; then
	mkdir -p "${IPA_TARGET_PATH}"
fi


#echo $PROJECT_FILE
#echo ${BUILD_TARGET}
#echo ${BUILD_CFG}
#echo ${CODE_SIGN_IDENTITY}
#echo ${PROVISIONING_PROFILE}
echo $JB_PACK
#echo ${ARCHIVE_SCHEME}
#echo ${IPA_TARGET_PATH}
#echo ${ARCHIVE_METHOD}
#exit 0


#PROJECT_FILE="../ios/Unity-iPhone.xcodeproj"
BUILD_DIR="$(dirname $PROJECT_FILE)/output"
#BUILD_TARGET="Unity-iPhone"
#ARCHIVE_SCHEME="Unity-iPhone"
#BUILD_CFG="Release"
PRODUCTNAME="target"
#CODE_SIGN_IDENTITY="iPhone Distribution: Shanghai Jiang You Information Technology Company Limited"
#PROVISIONING_PROFILE="jjsylite"
#IPA_TARGET_PATH="./IPA"

#######################################################################

ArchivePath="${ArchivePath=$BUILD_DIR/${PRODUCTNAME}.xcarchive}"


#######################################################################


if [ ! -d $IPA_TARGET_PATH ];then
	mkdir -p $IPA_TARGET_PATH
fi

if [ ! -d $BUILD_DIR ];then
	mkdir -p $BUILD_DIR
fi

#if [ "$DEBUG_MODE" == "$TRUE" ]; then
#	echo "" > $BUILD_DIR/cmd.log
#fi

function DebugRun()
{
	local CMD="$*"
	if [ "$DEBUG_MODE" == "$TRUE" ]; then
		echo $CMD > $BUILD_DIR/cmd.log
	fi
	exec $CMD
}

# build
function BuildXCodeProject() 
{
	xcodebuild clean build -project "$PROJECT_FILE" -target "$BUILD_TARGET" -configuration $BUILD_CFG -sdk iphoneos CONFIGURATION_BUILD_DIR="$BUILD_DIR" CODE_SIGN_IDENTITY="$CODE_SIGN_IDENTITY" PROVISIONING_PROFILE="$PROVISIONING_PROFILE" PRODUCT_NAME="$PRODUCTNAME"
}

#快速打越狱包
# createJailbreakPackage /path/to/locate/ipa/file
function QuciklyJailbreakPackage()
{
	local ProductAppPath=$BUILD_DIR/${PRODUCTNAME}.app
	local PayloadDir="Payload"
	if [ -d $PayloadDir ]; then
		rmdir $PayloadDir
	fi
	mkdir $PayloadDir
	cp -R $ProductAppPath $PayloadDir
	zip -r $1/${PRODUCTNAME}.quickly.jb.ipa $PayloadDir
	rm -rf $PayloadDir
}

#QuciklyJailbreakPackage $IPA_TARGET_PATH

function ArchiveXCodeProject() 
{
	xcodebuild \
		archive \
		-project "$PROJECT_FILE" \
		-scheme "$ARCHIVE_SCHEME" \
		-configuration $BUILD_CFG \
		-sdk iphoneos \
		CONFIGURATION_BUILD_DIR="$BUILD_DIR" \
		CODE_SIGN_IDENTITY="$CODE_SIGN_IDENTITY" \
		PROVISIONING_PROFILE="$PROVISIONING_PROFILE" \
		ENABLE_BITCODE=NO \
			-archivePath "$ArchivePath" 2>&1 | xcpretty -sc
	local pipestatus=${PIPESTATUS[0]}
	if [ $pipestatus -ne 0 ]; then
		echo "ARCHIVE FAILED"
	else
		echo "ARCHIVE SUCCEEDED"
	fi
	return $pipestatus
}

#ArchiveXCodeProject


function Archive2IPA()
{	
   	xcodebuild -exportArchive -archivePath "$ArchivePath" -exportPath "$IPA_TARGET_PATH/${PRODUCTNAME}.$BUILD_CFG.${ARCHIVE_METHOD}" \
		-exportOptionsPlist $SHELL_PATH/${ARCHIVE_METHOD}.plist | xcpretty -sc
	local pipestatus=${PIPESTATUS[0]}
	if [ $pipestatus -ne 0 ]; then
		echo "EXPORT FAILED" 
	else
		echo "EXPORT SUCCEEDED"
	fi
	return $pipestatus
}


function doIOSPackage()
{
	
	local ALL_STEPS="$STEPS"
	for ((i=1;i<=${#ALL_STEPS};i++)) do
		local CURRENT_STEP=`echo $ALL_STEPS | cut -c$i`
		if [ "$CURRENT_STEP" == '1' ]; then
			if [ "$JB_PACK" == "$TRUE" ]; then
				BuildXCodeProject
			else
				ArchiveXCodeProject
				if [ $? -ne 0  ]; then
					echoerr "ios project archive error. sure you have right options."
					exit 1
				fi
			fi
		elif [ "$CURRENT_STEP" == '2' ]; then
			if [ "$JB_PACK" == "$TRUE" ]; then
				QuciklyJailbreakPackage $IPA_TARGET_PATH
			else
				Archive2IPA
				if [ $? -ne 0  ]; then
					echoerr "ios archive export error. check plist file for export again"
					exit 1
				fi
			fi
		fi
	done
}


doIOSPackage
