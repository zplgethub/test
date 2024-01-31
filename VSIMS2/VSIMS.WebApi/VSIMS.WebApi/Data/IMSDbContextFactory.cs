// // /*
// // * Copyright (c) 2017～2027, zpl.
// // * all rights reserved.
// // *
// // * 文件名称：
// // * 功能摘要：文件定义.
// // *
// // * 作    者：zpl
// // * 当前版本：1.0
// // * 创建日期：2024-01-26 16:52
// // *
// // */

using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace VSIMS.WebApi.Data;

/// <summary>
/// 该类可指定运行时数据库连接。
/// 也可不采用该类，ef会直接探测 Services.AddDbContext创建连接
/// </summary>
public class ImsDbContextFactory : IDesignTimeDbContextFactory<ImsDataContext>
//public class ImsDbContextFactory
{
    public ImsDbContextFactory()
    {
    }

    public ImsDataContext CreateDbContext(string[] args)
    {
        string connectString =
            "Data Source=localhost;port=3306;Initial Catalog=CLMS;uid=root;password=zpl123;Charset=utf8;";
        Console.Out.WriteLine($"constr:{connectString}");
        var optionsBuilder = new DbContextOptionsBuilder<ImsDataContext>();
        optionsBuilder.UseMySql(connectString, ServerVersion.AutoDetect(connectString));

        return new ImsDataContext(optionsBuilder.Options);
    }
}