// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: ServerMessages.proto

#pragma warning disable 1591, 0612, 3021
namespace Protocol.Servers.Messages
{

    [global::ProtoBuf.ProtoContract()]
    public partial class Request_WorkTickets : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Request_WorkTickets()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public int stationid { get; set; }

        [global::ProtoBuf.ProtoMember(2, IsRequired = true)]
        public int playerid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Respond_WorkTickets : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Respond_WorkTickets()
        {
            data = new global::System.Collections.Generic.List<global::Protocol.DB.Table.WTI_Class_One>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<global::Protocol.DB.Table.WTI_Class_One> data { get; private set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Request_SubstationList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Request_SubstationList()
        {
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1, IsRequired = true)]
        public int uid { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class Respond_SubstationList : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
        {
            return global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);
        }
        public Respond_SubstationList()
        {
            list = new global::System.Collections.Generic.List<global::Protocol.RawData.StationData>();
            OnConstructor();
        }

        partial void OnConstructor();

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<global::Protocol.RawData.StationData> list { get; private set; }

    }

}

#pragma warning restore 1591, 0612, 3021