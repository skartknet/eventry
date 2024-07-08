﻿namespace Eventry
{
    public static class Constants
    {
        public static class ContentTypes
        {
            public static class Aliases
            {
                public const string BaseFolder = "eventryBaseFolder";
                public const string PhysicalEvent = "eventryPhysicalEvent";
                public const string OnlineEvent = "eventryOnlineEvent";
            }

            public static class Guids
            {
                public static readonly Guid BaseFolder = new("643478d7-536d-4bda-9d8c-baa378fdcf90");
                public static readonly Guid PhysicalEvent = new("d09c9116-48cf-4680-bf98-c114c4a69bc4");
                public static readonly Guid OnlineEvent = new("967fc6bf-d02a-49af-8580-faae9cee4a2b");
            }

        }

        public static class DataTypes
        {
            public static class Guids
            {
                public static readonly Guid Tags = new("242cd138-211c-4ba5-9629-3acda8f6bb1b");
                public static readonly Guid Maps = new("b184405d-435c-4597-819c-ad43133b288b");
            }
        }
    }
}