#!/bin/bash

SHELL_PATH=$( cd "$(dirname "$0")"; pwd)
PY_OUT=$SHELL_PATH/../../../GameServer/Messages/
ProtoPath=$SHELL_PATH/../Protocals
#protoc -I$ProtoPath --python_out=${PY_OUT} $ProtoPath/Message.proto
protoc -I$ProtoPath --python_out=${PY_OUT} $ProtoPath/**/*.proto
protoc -I$ProtoPath --python_out=${PY_OUT} $ProtoPath/*.proto
