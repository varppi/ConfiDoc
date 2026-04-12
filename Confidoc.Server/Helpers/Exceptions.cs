using Microsoft.AspNetCore.Mvc;

namespace Confidoc.Server.Helpers
{
    public static class Exceptions
    {
        public static Exception Null = new Exception("ran into an unexpected null value");
    }
}
