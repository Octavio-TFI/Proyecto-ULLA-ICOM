using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Database.Embeddings
{
    internal class SQLiteExtensionInterceptor
        : DbConnectionInterceptor
    {
        public override DbConnection ConnectionCreated(
            ConnectionCreatedEventData eventData,
            DbConnection result)
        {
            if(eventData.Connection is not SqliteConnection connection)
            {
                return base.ConnectionCreated(eventData, result);
            }

            connection.EnableExtensions();
            connection.LoadExtension("vec0");

            return connection;
        }
    }
}
