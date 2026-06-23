namespace API.Seguridad.Infrastructure.Audit
{
    public class Enumerations
    {
        public class Audits
        {
            public enum AuditEventType
            {
                Login,
                Logout,
                FailedLogin,
                PasswordChange,
                UserCreated,
                UserDeleted,
                EntityCreated,
                EntityUpdated,
                EntityDeleted,
                TwoFAVerified
            }

            public enum EntityActionType
            {
                Created,
                Updated,
                Deleted,
                None
            }

            public enum EntityType
            {
                User,
                Group,
                Role,
                Proceso,
            }   
        }
    }
}
