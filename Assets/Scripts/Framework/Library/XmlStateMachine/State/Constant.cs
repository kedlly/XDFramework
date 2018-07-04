

namespace Framework.Library.XMLStateMachine
{
	internal class StateConstant
	{
		internal const string XML_TAG_CATEGORY = "state";
		internal const string XML_TAG_NAME = "name";
		internal const string XML_TAG_PROPERTY_IS_ENTRY = "isEntry";
		internal const string XML_TAG_PROPERTY_IS_EXIT = "isExit";
		internal const string XML_TAG_PROPERTY_IS_REENTERABLE = "reenterable";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONENTRY = "onEnter";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONENTRY_NAME = XML_TAG_NAME;
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONEXIT = "onExit";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONEXIT_NAME = XML_TAG_NAME;
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONUPDATE = "onUpdate";
		internal const string XML_TAG_EXECUTABLE_BLOCK_ONUPDATE_NAME = XML_TAG_NAME;
		internal const string XML_TAG_TAG_KEY = "tag";
		internal const string XML_TAG_CONDUIT_VALUE = "conduit";

		internal const string ParseError_NoName_FMT = "xml parser find state with no state name error in state: {0}, may be fsm not working in runtime .";
	}

}