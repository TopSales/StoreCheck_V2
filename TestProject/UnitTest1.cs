using ZPF.SQL;
using System.Diagnostics;
using ZPF;

namespace TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DB3toFixedLength()
        {
            string dbFileName = @"D:\GitWare\TopSales\StoreCheck.V2\Terminal.MAUI\Resources\Raw\norma.db3";

            if (System.IO.File.Exists(dbFileName))
            {
                var _DBSQLViewModel = new DBSQLViewModel(new MSSQLiteEngine());
                string connectionString = DB_SQL.GenConnectionString(DBType.SQLite, "", dbFileName, "", "");

                try
                {
                    _DBSQLViewModel.Open(connectionString, true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                };

                var list = DB_SQL.Query<EAN_Article>(_DBSQLViewModel, "select EAN, Brand, Label_FR, Condi, UCondi, Price from EAN_Article order by EAN");

                if (string.IsNullOrEmpty(DB_SQL._ViewModel.LastError))
                {
                    string file = "";
                    long ind = 1;
                    long recSize = -1;

                    foreach (var art in list)
                    {
                        Debug.WriteLine($" {ind}/{list.Count}  {recSize}");

                        art.Brand = (art.Brand == null ? "" : art.Brand);
                        art.Condi = (art.Condi == null ? "" : art.Condi);
                        art.UCondi = (art.UCondi == null ? "" : art.UCondi);

                        file += art.EAN.PadRight(13).Right(13)
                             + art.Brand.PadRight(32).Right(32)
                             + art.Label_FR.PadRight(64).Right(64)
                             + art.Condi.PadRight(8).Right(8)
                             + art.UCondi.PadRight(8).Right(8)
                             + art.Price.ToString().PadRight(8).Right(8);

                        if (ind == 1)
                        {
                            recSize = file.Length;
                        };

                        ind++;
                    };

                    System.IO.File.WriteAllText(dbFileName + ".txt", file, System.Text.Encoding.ASCII);
                }
                else
                {
                    // DB_SQL._ViewModel.LastError == “SQLite Error 14: 'unable to open database file'.”
                    // https://github.com/xamarin/xamarin-android/issues/3819

                    Debug.WriteLine(DB_SQL._ViewModel.LastError);
                    Debugger.Break();
                };
            }
            else
            {
                Debugger.Break();
            };
        }
    }
}