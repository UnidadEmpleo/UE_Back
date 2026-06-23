using System.Data.SqlTypes;

namespace API.Seguridad.Application.Core
{
    public struct AzureAD
    {
        public struct User
        {
            public const int DefaultInactiveTime = 30;
        }
        public struct AddtionalData
        {
            public const string FechaActualizacion = "FechaActualizacion";
            public const string Activo = "Activo";
            public const string InactiveTime = "inactiveTime";
            public const string LastName = "lastName";
            public const string Corporaciones = "corporaciones";
        }

        public struct Configuration
        {
            public const string Domain = "AzureAd:Domain";
            public const string ClientId = "AzureAd:ClientId";
            public const string TenantId = "AzureAd:TenantId";
            public const string ClientSecret = "AzureAd:ClientSecret";
            public const string Instance = "AzureAd:Instance";
            public const string GraphUrl = "AzureAd:GraphUrl";
            public const string ApplicationId = "AzureAd:ApplicationId";
            public const string ServicePrincipalId = "AzureAd:ServicePrincipalId";
        }
    }
}
