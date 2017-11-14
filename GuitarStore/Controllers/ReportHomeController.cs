using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using GuitarStore.Areas.Reports.Models;

namespace GuitarStore.Controllers
{
    public class ReportHomeController : Controller
    {
        // GET: Reports/ReportIndex
        public ActionResult Index(string selectedState = "California")
        {
            //Let's work with ADO.Net - High Ceremony, but high-level of control

            SalesReportModel model = new SalesReportModel();

            //Look up a connection string by name out of web.config
            string connectionString = ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //Open the doorway to the server
                connection.Open();

                //Commands are used to send a statement to SQL
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT DISTINCT StateProvince FROM Address INNER JOIN SalesOrderHeader ON Address.AddressID = SalesOrderHeader.BillToAddressID";
                //If you're doing an update, delete, or insert, just send the query over:
                //command.ExecuteNonQuery()

                //Execute scalar is used if your query gives back one single value (e.g. one row with one columns)
                //command.ExecuteScalar()

                //Use execute reader to read data in, record by record
                List<string> states = new List<string>();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        states.Add(reader.GetString(0));
                    }
                }

                model.States = states.ToArray();

                //TODO: Get these using ADO.Net

                model.TopSalesByDollar = new TopSaleByDollar[0];
                SqlCommand dollarCommand = connection.CreateCommand();
                dollarCommand.CommandText =
                @"select top 5 ProductID, SUM(OrderQty) from salesorderdetail JOIN SalesOrderHeader
                                        ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID
                                        JOIN[Address] ON SalesOrderHeader.BillToAddressID = Address.AddressID
                                         WHERE Address.StateProvince = '" + selectedState + "' group by ProductID order by sum(LineTotal) DESC";

                List<TopSaleByDollar> dollar = new List<TopSaleByDollar>();
                using (SqlDataReader dollarReader = dollarCommand.ExecuteReader())
                {
                    while (dollarReader.Read())
                    {
                        dollar.Add(new TopSaleByDollar { ProductID = dollarReader.GetInt32(0), Price = dollarReader.GetInt32(1) });
                    }
                    model.TopSalesByDollar = dollar.ToArray();
                }

                model.TopSalesByQuantity = new TopSaleByQuantity[0];
                SqlCommand quantityCommand = connection.CreateCommand();
                quantityCommand.CommandText =
                @"SELECT TOP 5 ProductID, SUM(OrderQty) from salesorderdetail JOIN SalesOrderHeader
                                        ON SalesOrderDetail.SalesOrderID = SalesOrderHeader.SalesOrderID
                                        JOIN[Address] ON SalesOrderHeader.BillToAddressID = Address.AddressID
                                         WHERE Address.StateProvince = '" + selectedState + "' group by ProductID order by sum(OrderQty) DESC";

                List<TopSaleByQuantity> quantity = new List<TopSaleByQuantity>();
                using (SqlDataReader quantityReader = quantityCommand.ExecuteReader())
                {
                    while (quantityReader.Read())
                    {
                        quantity.Add(new TopSaleByQuantity { ProductID = quantityReader.GetInt32(0), Quantity = quantityReader.GetInt32(1) });
                    }
                    model.TopSalesByQuantity = quantity.ToArray();

                    //Make sure you close the connection when you're finished
                    connection.Close();
                }
                return View(model);
            }
        }
    }
}