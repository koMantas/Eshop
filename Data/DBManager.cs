using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;



namespace Eshop.Data
{
    public class DbManager
    {
        public Pirkejas Authentification(string email)
        {
            using (var db = new EshopDBContext())
            {
                return db.Pirkejas.Where(i => i.El_pastas == email).FirstOrDefault();
            }
        }

        public void RegisterUser(string firstName, string lastName, string email, string telNum, string city, string street, string houseNum, string apartmentNum)
        {
            using (var db = new SqlConnection(Eshop.Settings.Default.ConnectionStringDB))
            {
                db.Open();

                //INSERT COMMAND
                SqlCommand cmd = new SqlCommand("INSERT INTO Pirkejas (Vardas,Pavarde,Tel_NR,El_pastas,Miestas,Gatve,Namas,Butas)" +
                                                " VALUES (@firstName,@lastName,@telNum,@email,@city,@street,@houseNum,@apartmentNum)");
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@telNum", telNum);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@city", city);
                cmd.Parameters.AddWithValue("@street", street);
                cmd.Parameters.AddWithValue("@houseNum", houseNum);
                cmd.Parameters.AddWithValue("@apartmentNum", apartmentNum);

                cmd.Connection = db;

                cmd.ExecuteNonQuery();

            }
        }

        public DataTable GetUserOders(Pirkejas logedBuyer)
        {
            using (var db = new SqlConnection(Eshop.Settings.Default.ConnectionStringDB))
            {
                db.Open();

                //DATA ADAPTER ir DATATABLE
                //SELECT command
                SqlDataAdapter data = new SqlDataAdapter();
                SqlCommand command = new SqlCommand("SELECT A.Nr, A.Priemimo_data, A.Sandeliavimo_data, A.Atidavimo_data, SUM(Kiekis * Kaina) AS Moketi" +
                                                    " FROM Uzsakymas A, Uzsakymo_info B " +
                                                    " WHERE A.Nr = B.Uzsakymas_ID AND A.Pirkejas = @UserID" +
                                                    " GROUP BY A.Nr, A.Priemimo_data, A.Sandeliavimo_data, A.Atidavimo_data;", db);
                command.Parameters.AddWithValue("@UserID", logedBuyer.Nr);

                data.SelectCommand = command;

                DataTable userOrders = new DataTable();
                data.Fill(userOrders);
                data.Dispose();
                return userOrders;
            }
        }

        public DataTable GetOrderInfo(Pirkejas user, int id)
        {
            Uzsakymas order;
            using (var db = new EshopDBContext())
            {
                order = db.Uzsakymas.Where(i => i.Nr == id).FirstOrDefault();
            }
            if (order != null)
            {
                using (var db = new SqlConnection(Eshop.Settings.Default.ConnectionStringDB))
                {
                    db.Open();

                    SqlDataAdapter data = new SqlDataAdapter();
                    SqlCommand command = new SqlCommand(" SELECT C.Pavadinimas,C.Gamintojas, B.Kiekis, B.Kaina" +
                                                        " FROM Uzsakymas A, Uzsakymo_info B, Preke C" +
                                                        " WHERE A.Nr = B.Uzsakymas_ID AND B.Preke_ID = C.ID AND A.Pirkejas = @UserID AND A.Nr = @OrderID;", db);
                    command.Parameters.AddWithValue("@UserID", user.Nr);
                    command.Parameters.AddWithValue("@OrderID", order.Nr);

                    data.SelectCommand = command;

                    DataTable orderInfo = new DataTable();
                    data.Fill(orderInfo);
                    data.Dispose();
                    return orderInfo;
                }
            }
            else
                return null;
        }

        public List<IGrouping<string, ProductInfo>> GetProductsInfo()
        {
            using (var db = new EshopDBContext())
            {
                //LINQ Join ir groupBy
                //SELECT entity
                var productsInfo = db.Preke.Join(db.Kategorija,
                                                 e => e.Kategorija1.Nr,
                                                 s => s.Nr,
                                                 (e, s) => new ProductInfo
                                                 {
                                                     ID = e.ID,
                                                     Pavadinimas = e.Pavadinimas,
                                                     Gamintojas = e.Gamintojas,
                                                     Garantija = e.Garantija,
                                                     Kaina = e.Kaina,
                                                     Kategorija = s.Pavadinimas,
                                                     Liko = e.Liko,
                                                     Nuolaida = s.Nuolaida,
                                                     KainaSuNuolaida = e.Kaina * (1 - (s.Nuolaida / 100))
                                                 }).GroupBy(e => e.Kategorija);

                return productsInfo.ToList();
            }
        }

        public Uzsakymas CreateOrder(Pirkejas logedUser)
        {
            var newOrder = new Uzsakymas();
            using (var db = new EshopDBContext())
            {
                //INSERT entity
                newOrder.Pirkejas = logedUser.Nr;
                db.Uzsakymas.Add(newOrder);
                db.SaveChanges();
                return newOrder;
            }
        }

        public void RemoveOrder(Uzsakymas order)
        {
            List<Uzsakymo_info> orderInfo;
            using (var db = new EshopDBContext())
            {
                orderInfo = db.Uzsakymo_info.Where(e => e.Uzsakymas_ID == order.Nr).ToList();
            }
            using (var dbConnection = new SqlConnection(Eshop.Settings.Default.ConnectionStringDB))
            {
                dbConnection.Open();

                foreach (var singleOrderInfo in orderInfo)
                {
                    //UPDATE command
                    SqlCommand cmd1 = new SqlCommand("UPDATE Preke SET Liko = Liko + @Quantity WHERE ID = @ProductID");
                    cmd1.Connection = dbConnection;
                    cmd1.Parameters.AddWithValue("@Quantity", singleOrderInfo.Kiekis);
                    cmd1.Parameters.AddWithValue("@ProductID", singleOrderInfo.Preke_ID);

                    cmd1.ExecuteNonQuery();
                }
                //DELETE command
                SqlCommand cmd2 = new SqlCommand();
                cmd2.CommandText = "DELETE FROM Uzsakymo_info WHERE Uzsakymas_ID = @OrderID";
                cmd2.Connection = dbConnection;
                cmd2.Parameters.AddWithValue("@OrderId", order.Nr);

                cmd2.ExecuteNonQuery();
            }

            using (var db = new EshopDBContext())
            {
                //DELETE entity
                db.Uzsakymas.Attach(order);
                db.Uzsakymas.Remove(order); //removerange
                db.SaveChanges();
            }
        }

        public bool AddProductToOrder(Uzsakymas order, int productId, int quantity)
        {
            using (var db = new EshopDBContext())
            {
                var product = db.Preke.Where(e => e.ID == productId).FirstOrDefault();
                if (product != null && quantity >= 0)
                {
                    var orderedProduct = db.Uzsakymo_info.Where(e => e.Preke_ID == productId).Where(e => e.Uzsakymas_ID == order.Nr).FirstOrDefault();
                    var category = db.Kategorija.Where(e => e.Nr == product.Kategorija).FirstOrDefault();
                    if (orderedProduct == null && quantity != 0)
                    {
                        if (product.Liko < quantity)
                            return false;
                        else
                        {
                            var uzsakymoInfo = new Uzsakymo_info { Preke_ID = product.ID, Uzsakymas_ID = order.Nr, Kiekis = quantity };
                            uzsakymoInfo.Kaina = product.Kaina * (1 - ((decimal)category.Nuolaida / 100));
                            product.Liko -= quantity;
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.Uzsakymo_info.Add(uzsakymoInfo);
                            db.SaveChanges();
                            return true;
                        }
                    }
                    if (orderedProduct != null)
                    {
                        int difference = quantity - orderedProduct.Kiekis;
                        //wants less products
                        if (quantity == 0)
                        {
                            db.Uzsakymo_info.Remove(orderedProduct);
                            db.SaveChanges();
                            return true;
                        }
                        else if (difference < 0)
                        {
                            orderedProduct.Kiekis = quantity;
                            product.Liko += Math.Abs(difference);
                            db.Entry(orderedProduct).State = System.Data.Entity.EntityState.Modified;
                            db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            return true;

                        }
                        //wants more products
                        else
                        {
                            if (product.Liko < difference)
                                return false;
                            else
                            {
                                orderedProduct.Kiekis = quantity;
                                product.Liko -= Math.Abs(difference);
                                db.Entry(orderedProduct).State = System.Data.Entity.EntityState.Modified;
                                db.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                return true;
                            }
                        }
                    }
                }
                else
                    return false;
            }
            return false;
        }

        public decimal GetOrderSum(Uzsakymas order)
        {
            decimal sum;
            using (var db = new EshopDBContext())
            {
                //LINQ agregatine funkcija
                sum = db.Uzsakymo_info.Where(e => e.Uzsakymas.Nr == order.Nr).
                    Select(e => e.Kaina * e.Kiekis).Sum();
            }
            return sum;
        }

        public void EndOrder(Uzsakymas order)
        {
            order.Priemimo_data = DateTime.Today;
            using (var db = new EshopDBContext())
            {
                //UPDATE entity
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}
