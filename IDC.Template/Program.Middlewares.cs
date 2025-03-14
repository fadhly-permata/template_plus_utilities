internal partial class Program
{
    private static void ConfigureMiddlewares(WebApplication app)
    {
        ConfigureSwaggerUI(app: app);
        app.UseHttpsRedirection();
        ConfigureStaticFiles(app: app);
        app.UseAuthorization();
        app.MapControllers();
    }
}
