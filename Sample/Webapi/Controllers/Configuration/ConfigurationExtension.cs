namespace Webapi.MyExtension
{
    public static class ConfigurationExtension
    {
        public static void SampleConfigurationForService(this WebApplicationBuilder builder, string[] args)
        {
            //增加新的环境变量前缀
            builder.Configuration.AddEnvironmentVariables("PREFIX_");
            //交换机映射,之后可以通过configuration的索引器获取参数
            var switchMappings = new Dictionary<string, string>()
                     {
                         { "--arg1", "arg1" },
                         { "--arg2", "arg2" },
                         { "--arg3", "arg3" }
                     };
            builder.Configuration.AddCommandLine(args, switchMappings);
            //增加xml文件
            builder.Configuration.AddXmlFile("MyXml.xml", true);
            //增加ini文件
            builder.Configuration.AddIniFile("MyIni.ini", true, reloadOnChange: true);
            builder.Configuration.AddJsonFile("settings.json");


            //变更令牌
            //var confiuration = (IConfiguration)builder.Configuration;
            //ChangeToken.OnChange(() => confiuration.GetReloadToken(), () =>
            //{
            //    Console.WriteLine("has changed");
            //});
        }
    }
}
