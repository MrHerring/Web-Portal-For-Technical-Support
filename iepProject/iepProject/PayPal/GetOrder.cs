using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

using BraintreeHttp;
using iepProject.Models;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;

namespace iepProject.PayPal
{
    public class GetOrderClass
    {    //2. Set up your server to receive a call from the client
         /*
           You can use this method to retrieve an order by passing the order ID.
          */

        public static int Silver = 5;
        public static String SilverPrice = "5.00";
        public static int Gold = 15;
        public static String GoldPrice = "15.00";
        public static int Platinum = 25;
        public static String PlatinumPrice = "25.00";

        public static HttpResponse GetOrder(string orderId, int userId, bool debug = true)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            //3. Call PayPal to get the transaction
            var response = Task.Run(() => PayPalClient.client().Execute(request)).Result;
            //4. Save the transaction in your database. Implement logic to save transaction to your database for future reference.
            var result = response.Result<Order>();
            String value = result.PurchaseUnits[0].AmountWithBreakdown.Value;
            
            /*if (result.Status != "COMPLETE") {
                return response;
            }*/

            Transaction transaction = new Transaction();

            transaction.Status = result.Status;
            transaction.Amount = value;
            transaction.UserId = userId;
            ApplicationContext db = new ApplicationContext();

            User user = db.Users.SingleOrDefault(u => u.Id == userId);

            if (String.Compare(value,SilverPrice) == 0)
            {
                
                user.TokenAmount += Silver;
                transaction.Package = "Silver";
            }

            if (String.Compare(value,GoldPrice) == 0)
            {
                
                user.TokenAmount += Gold;
                transaction.Package = "Gold";
            }

            if (String.Compare(value, PlatinumPrice) == 0)
            {
               
                user.TokenAmount += Platinum;
                transaction.Package = "Platinum";
            }

            transaction.Paid = float.Parse(transaction.Amount);

            db.Entry(user).State = EntityState.Modified;
            db.Transactions.Add(transaction);
            db.SaveChanges();

            String amount = result.PurchaseUnits[0].AmountWithBreakdown.Value;

            return response;
        }


        static void Main(string[] args)
        {
            
        }
    }
}
    