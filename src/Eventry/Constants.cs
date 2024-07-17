namespace Eventry
{
    public static class Constants
    {
        public static class ContentTypes
        {
            public static class Aliases
            {
                public const string EventBaseComposition = "eventryEventBaseComposition";
                public const string PhysicalEvent = "eventryPhysicalEvent";
                public const string OnlineEvent = "eventryOnlineEvent";
                public const string EventListing = "eventryEventListing";
            }

            public static class Guids
            {
                public static readonly Guid BaseFolder = new("643478d7-536d-4bda-9d8c-baa378fdcf90");
                public static readonly Guid EventBaseComposition = new("45a4e210-c226-42ba-8258-50d3b19a4276");
                public static readonly Guid PhysicalEvent = new("d09c9116-48cf-4680-bf98-c114c4a69bc4");
                public static readonly Guid OnlineEvent = new("967fc6bf-d02a-49af-8580-faae9cee4a2b");
                public static readonly Guid EventsListing = new("f6f00e22-d3bf-4fa1-b658-55687307bf7b");
            }

        }

        public static class DataTypes
        {
            public static class Guids
            {
                public static readonly Guid Tags = new("242cd138-211c-4ba5-9629-3acda8f6bb1b");
                public static readonly Guid Maps = new("b184405d-435c-4597-819c-ad43133b288b");
                public static readonly Guid Price = new("84b8d092-4335-4a20-b591-4a1b22305e81");
                public static readonly Guid Stock = new("4a78c330-2f42-4278-bd54-1ff091b16e0c");
            }
        }
    }
}
