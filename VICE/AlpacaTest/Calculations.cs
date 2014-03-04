using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlpacaFinal
{
    public class Calculations
    {
        //Inputs
        public float hours = 0;
        public float kilowatts = 0;
        public float power = 0;
        public float KW = 0;

        //Intermediate
        float[] tierRange = new float[4] { 0f, 300f, 400f, 600f };
        float[] tierPricing = new float[4] { 0.14f, 0.16f, 0.29f, 0.31f };
        float[] tierCost = new float[4] { 0.0f, 0.0f, 0.0f, 0.0f };
        //float[] KW = new float[4] { 400.0f, 800.0f, 0.0f, 400.0f };

        //Outputs
        private float cost = 0;

        public float Cost
        {
            //Read only - no setting outside of the calculation
            get { return cost; }
        }

        public void Calculate()
        {
            //Initalize tier cost values
            for (int i=0; i<4; i++)
            {
                tierCost[i] = 0.0f;
            }

            if (kilowatts < tierRange[1])
            {
                //First tier only
                tierCost[0] = kilowatts * tierPricing[0];
            }
            else if (kilowatts < tierRange[2])
            {
                //Second tier
                tierCost[0] = tierRange[1] * tierPricing[0];
                tierCost[1] = (kilowatts - tierRange[1]) * tierPricing[1];
            }
            else if (kilowatts < tierRange[3])
            {
                //Third tier
                tierCost[0] = tierRange[1] * tierPricing[0];
                tierCost[1] = tierRange[2] * tierPricing[1];
                tierCost[2] = (kilowatts - tierRange[2]) * tierPricing[2];
            }
            else
            {
                //Fourth tier
                tierCost[0] = tierRange[1] * tierPricing[0];
                tierCost[1] = tierRange[2] * tierPricing[1];
                tierCost[2] = tierRange[3] * tierPricing[2];
                tierCost[3] = (kilowatts - tierRange[3]) * tierPricing[3];
            }

            cost = 0;
            for (int i = 0; i < 3; i++)
            {
                cost += tierCost[i];
            }
        }
    }
}
