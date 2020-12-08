using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DroneWebsocket
{
    class DroneData
    {
        //Id    |Latitude          |Longitude         |UTC                     |Altitude Meters   |Temperature C     |Pressure          |Humidity          |Wind Speed       |Wind Direction    |Percent Cloud
        //564677|-21.55865276355627|-6416.398563694417|2019-08-01T10:16:00.000Z|11749.064339481984|22.806486599997378|1020.1773310249765|103.94502797708331|93.68961149420538|271.81955742678406|11.462058016340503 
        private int id;
        private double latitude;
        private double longitude;
        private DateTime utc_time;
        private double altitude;
        private double temperature;
        private double pressure;
        private double humidity;
        private double windSpeed;
        private double windDirection;
        private double percentCloud;

        //KPI's to Display include Timestamp, temperature, pressure, percent cloud, humidity
        public List<Double> TemperatureList;
        public List<Double> PressureList;
        public List<Double> PercentCloudList;
        public List<Double> HumidityList;


        //Nearly 1000 messages will stream a minute so for five minutes we need 5 thousand historical entries.
        private const int SizeOfArrayForFiveMinutes = 1000 * 5;

        public string Id { get { return String.Format("{0}", id); } } //No extra formatting 
        public string Latitude { get { return String.Format("{0:F3}", latitude); } } //Format to 3 decimal places
        public string Longitude { get { return String.Format("{0:F1}", longitude); } } //Format to one decimal place
        public string UTC_time { get { return String.Format("{0:T}", utc_time); } } //Format to time only
        public string Altitude { get { return String.Format("{0:F1}", altitude); } } //Format to one decimal place
        public string Temperature { get { return String.Format("{0:F1}", temperature); } } //Format to one decimal place
        public string Humidity { get { return String.Format("{0:F1}", humidity); } } //Format to one decimal place
        public string Pressure { get { return String.Format("{0:F1}", pressure); } } //Format to one decimal place
        public string WindSpeed { get { return String.Format("{0:F1}", windSpeed); } } //Format to one decimal place
        public string WindDirection { get { return String.Format("{0:F1}", windDirection); } } //Format to one decimal place
        public string PercentCloud { get { return String.Format("{0:F1}", percentCloud); } } //Format to one decimal place

        public DroneData()
        {
            //Initilize the lists to be used for averages so they are not null
            TemperatureList = new List<double>();
            PressureList = new List<double>();
            PercentCloudList = new List<double>();
            HumidityList = new List<double>();
        }

        public void ParseData(string rawData)
        {
            try
            {
                string[] splitData = rawData.Split('|');
                id = Convert.ToInt32(splitData[0]);
                latitude = Convert.ToDouble(splitData[1]);
                longitude = Convert.ToDouble(splitData[2]);
                try
                {
                    utc_time = DateTime.Parse(splitData[3], null, DateTimeStyles.RoundtripKind);
                }
                catch (FormatException)
                {
                    Console.WriteLine("{0} is not in the correct format.", splitData[3]);
                }
                altitude = Convert.ToDouble(splitData[4]);
                temperature = Convert.ToDouble(splitData[5]);
                TemperatureList.Add(temperature);//Add this to a list to be used for averaging

                pressure = Convert.ToDouble(splitData[6]);
                PressureList.Add(pressure);//Add this to a list to be used for averaging

                humidity = Convert.ToDouble(splitData[7]);
                HumidityList.Add(humidity);//Add this to a list to be used for averaging

                windSpeed = Convert.ToDouble(splitData[8]);
                windDirection = Convert.ToDouble(splitData[9]);
                percentCloud = Convert.ToDouble(splitData[10]);
                PercentCloudList.Add(percentCloud);//Add this to a list to be used for averaging
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public string GetTemperatureAverage()
        {
            string average = ""; //Set to a blank string to start

            if (TemperatureList.Count < 1) return ""; //If list is empty return a blank string

            while (TemperatureList.Count > SizeOfArrayForFiveMinutes)
            {
                TemperatureList.RemoveAt(0);//Remove the oldest index in the array to keep the size of the array always set to be five minutes worth
            }

            try
            {
                average = String.Format("{0:F1}", TemperatureList.Average());//Return the average from the latest five minutes of  //Format to one decimal place
            }
            catch { } //Sometimes a member is added to the list and trying to take the average will cause error. Do nothing with the catch.

            return average;
        }

        public string GetPressureAverage()
        {
            string average = ""; //Set to a blank string to start

            if (PressureList.Count < 1) return average; //If list is empty return a blank string

            while (PressureList.Count > SizeOfArrayForFiveMinutes)
            {
                PressureList.RemoveAt(0);//Remove the oldest index in the array to keep the size of the array always set to be five minutes worth
            }

            try
            {
                average = String.Format("{0:F1}", PressureList.Average());//Return the average from the latest five minutes of data //Format to one decimal place
            }
            catch { } //Sometimes a member is added to the list and trying to take the average will cause error. Do nothing with the catch.

            return average;
        }

        public string GetHumidityAverage()
        {
            string average = ""; //Set to a blank string to start

            if (HumidityList.Count < 1) return ""; //If list is empty return a blank string

            while (HumidityList.Count > SizeOfArrayForFiveMinutes)
            {
                HumidityList.RemoveAt(0);//Remove the oldest index in the array to keep the size of the array always set to be five minutes worth
            }

            try
            {
                average = String.Format("{0:F1}", HumidityList.Average());//Return the average from the latest five minutes of data //Format to one decimal place
            }
            catch { } //Sometimes a member is added to the list and trying to take the average will cause error. Do nothing with the catch.

            return average;
        }

        public string GetPercentCloudAverage()
        {
            string average = ""; //Set to a blank string to start

            if (PercentCloudList.Count < 1) return ""; //If list is empty return a blank string

            while (PercentCloudList.Count > SizeOfArrayForFiveMinutes)
            {
                PercentCloudList.RemoveAt(0);//Remove the oldest index in the array to keep the size of the array always set to be five minutes worth
            }

            try
            {
                average = String.Format("{0:F1}", PercentCloudList.Average());//Return the average from the latest five minutes of data //Format to one decimal place
            }
            catch { } //Sometimes a member is added to the list and trying to take the average will cause error. Do nothing with the catch.

            return average;
        }
    }

}
