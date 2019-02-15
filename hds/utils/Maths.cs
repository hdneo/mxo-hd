using System;
using System.Collections;
namespace hds
{
	public class Maths
	{
		// Constant values inside mxo
		private const float walkingSpeed=0.176f;
		private const float runningSpeed=0.607f;
		private const float degreesPerByte=1.41f;
        private Random rand = new Random();
		private const int maxDegrees = 255;
		
		public Maths ()
		{
		}
		
		
		public float currentpos(float init, float destination,int millisecsPassed){
			
			if (init> destination)
				return init-(walkingSpeed*millisecsPassed);
			else 
				return init+(walkingSpeed*millisecsPassed);
		}
		
		public byte getByteForDegrees(int degrees){
			if (degrees>maxDegrees)
				degrees -=maxDegrees;
			
			byte result = (byte)(0x00+degrees);
			return result;
		} 
		
		public int getOppositeDegrees(int actual){
			int opposite = (actual+128);
			if (opposite>maxDegrees)
				opposite -=maxDegrees;
			return opposite;
		}
		
		public float distance2Coords(float x1,float x2,float z1,float z2){
			
			return (float)Math.Sqrt(((x2-x1)*(x2-x1)) + ((z2-z1)*(z2-z1)));
		}

        public bool IsInCircle(float src_x, float src_z, float otherpoint_x , float otherpoint_z, float radius)
        {
            return (Math.Pow((src_x - otherpoint_x), 2) + Math.Pow(src_z - otherpoint_z, 2)) <= radius * radius;
        }

        public int diceRollOperant()
        {
            Random rand = new Random();
            int dice = rand.Next(1, 3);

            return dice;
        }

        public LtVector3f RandomPointInCircleV2(float xCurrent, float zCurrent, float radius)
        {
            // new vector
            LtVector3f newPos = new LtVector3f();
            newPos.a = 0.0f;
            newPos.c = 0.0f;

            int u = this.rand.Next(1,32000);
            int v = this.rand.Next(1,32000);
            double theta = 2 * Math.PI * u;
            var phi = Math.Acos(2 * v - 1);
            double x = xCurrent + (radius * Math.Sin(phi) * Math.Cos(theta));
            double y = 95.0;
            double z = zCurrent + (radius * Math.Cos(phi));
            newPos.a = (float)x;
            newPos.b = (float)y;
            newPos.c = (float)z;
            return newPos;
        }


        public float RandomBetween(float min, float max)
        {
            return min + (float)rand.NextDouble() * (max - min);
        }

        // ToDo: Implement the methods from the file..it could be the paradise^^
        public LtVector3f RandomPointOnCircle(float xCurrent, float yCurrent, float zCurrent, float radius)
        {
            double angle = (double)RandomBetween(0.0f, (float)Math.PI*2);
            LtVector3f newPos = new LtVector3f();
            newPos.a = xCurrent + radius * (float)Math.Cos(angle);
            newPos.b = yCurrent;
            newPos.c = zCurrent + radius * (float)Math.Sin(angle);

            return newPos;
        }
		
		public static float getDistance(float x1,float y1,float z1,float x2,float y2, float z2)
		{
			float deltaX = x1 - x2;
			float deltaY = y1 - y2;
			float deltaZ = z1 - z2;
			float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
			return distance;
		}

        public LtVector3f RandomPointInCircle2D(float xCurrent, float zCurrent, float radius)
        {
            // calc http://stackoverflow.com/questions/5531827/random-point-on-a-given-sphere
            // http://freespace.virgin.net/hugo.elias/routines/r_dist.htm - Distance
            LtVector3f newPos = new LtVector3f();
            newPos.a = 0.0f;
            newPos.c = 0.0f;

            
            int randMax = 254;
            float angle = (float)(rand.Next(1, randMax) * (Math.PI * 2) / randMax);
            float distance = (float)Math.Sqrt(rand.Next(1, (int)radius) * 1.0 / randMax) * radius / 100;

            
            float x = (float)Math.Cos(angle) * distance + (xCurrent/100);
            float z = (float)Math.Sin(angle) * distance + (zCurrent/100);

            Output.WriteDebugLog("Distance : " + distance);

            newPos.a = x;
            newPos.c = z;
            return newPos;
        }

        public LtVector3f[] CalculateJumpPoints(float xCurrent, float yCurrent, float zCurrent, float xDestination, float yDestination, float zDestination, long maxHeight, int countParts, long timeEstimation)
        {
            LtVector3f[] Points = new LtVector3f[countParts+1];
            // ToDo: Calculate the positions between

            return Points;
        }
		
	}
}

