using System;
using System.Web.Http;

namespace LADocDbAPI.Controllers
{
    /// <summary>
    /// </summary>
    public class ConversionController : ApiController
    {
        /// <summary>
        ///     Convert DateTime to UnixTimestamp
        /// </summary>
        /// <param name="dateTime">DateTime value</param>
        /// <returns></returns>
        [Route("Unix/")]
        public double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double) unixTimeStampInTicks/TimeSpan.TicksPerSecond;
        }


        /// <summary>
        ///     Connvert UnixTimestamp to DateTime
        /// </summary>
        /// <param name="unixTime">double value</param>
        /// <returns></returns>
        [Route("DateTime/")]
        public DateTime UnixTimestampToDateTime(double unixTime)
        {
            var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var unixTimeStampInTicks = (long) (unixTime*TimeSpan.TicksPerSecond);
            return new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc);
        }
    }
}