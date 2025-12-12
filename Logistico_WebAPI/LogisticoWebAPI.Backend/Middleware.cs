namespace LogisticoWebAPI.Backend
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Protección contra Clickjacking (CWE-1021)
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Content Security Policy más estricta
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com https://fonts.googleapis.com; " +
                "font-src 'self' https://cdn.jsdelivr.net https://fonts.gstatic.com https://cdnjs.cloudflare.com; " +
                "img-src 'self' data: https: blob:; " +
                "connect-src 'self' https://localhost:* wss://localhost:* https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
                "frame-ancestors 'none'; " +
                "form-action 'self'");

            // Otros headers de seguridad importantes
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // HSTS para HTTPS
            if (context.Request.IsHttps)
            {
                context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");
            }

            await _next(context);
        }
    }
}