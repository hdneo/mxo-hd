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
            newPos.x = 0.0f;
            newPos.z = 0.0f;

            int u = this.rand.Next(1,32000);
            int v = this.rand.Next(1,32000);
            double theta = 2 * Math.PI * u;
            var phi = Math.Acos(2 * v - 1);
            double x = xCurrent + (radius * Math.Sin(phi) * Math.Cos(theta));
            double y = 95.0;
            double z = zCurrent + (radius * Math.Cos(phi));
            newPos.x = (float)x;
            newPos.y = (float)y;
            newPos.z = (float)z;
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
            newPos.x = xCurrent + radius * (float)Math.Cos(angle);
            newPos.y = yCurrent;
            newPos.z = zCurrent + radius * (float)Math.Sin(angle);

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
            // calc http://stackoverflow.com/questions/5531827/random-point-on-x-given-sphere
            // http://freespace.virgin.net/hugo.elias/routines/r_dist.htm - Distance
            LtVector3f newPos = new LtVector3f();
            newPos.x = 0.0f;
            newPos.z = 0.0f;

            
            int randMax = 254;
            float angle = (float)(rand.Next(1, randMax) * (Math.PI * 2) / randMax);
            float distance = (float)Math.Sqrt(rand.Next(1, (int)radius) * 1.0 / randMax) * radius / 100;

            
            float x = (float)Math.Cos(angle) * distance + (xCurrent/100);
            float z = (float)Math.Sin(angle) * distance + (zCurrent/100);

            Output.WriteDebugLog("Distance : " + distance);

            newPos.x = x;
            newPos.z = z;
            return newPos;
        }

        public LtVector3f[] CalculateJumpPoints(float xCurrent, float yCurrent, float zCurrent, float xDestination, float yDestination, float zDestination, long maxHeight, int countParts, long timeEstimation)
        {
            LtVector3f[] Points = new LtVector3f[countParts+1];
            // ToDo: Calculate the positions between

            return Points;
        }

        // https://gamedev.stackexchange.com/questions/133794/parabolic-movement-of-a-gameobject-in-unity
        public static LtVector3f[] ParabolicMovement(LtVector3f startingPos, LtVector3f arrivingPos, float maxHeight, int framesCount)
        {
            // float ANIMATION_DURATION = 2.0f;
            // float FRAMES_PER_SECOND = 30.0f;
            // int framesNum = (int)(ANIMATION_DURATION * FRAMES_PER_SECOND);
            int framesNum = framesCount;
            LtVector3f[] frames = new LtVector3f[framesNum];
        

            //PROJECTING ON Z AXIS
            LtVector3f stP = new LtVector3f(0, startingPos.y, startingPos.z);
            LtVector3f arP = new LtVector3f(0, arrivingPos.y, arrivingPos.z);

            LtVector3f diff = new LtVector3f((arP.x - stP.x)/ 2 + maxHeight, (arP.y - stP.y) / 2 + maxHeight, (arP.z - stP.z) / 2 + maxHeight);

            LtVector3f vertex = new LtVector3f(stP.x + diff.x, stP.y + diff.y, stP.z + diff.z);

            float x1 = startingPos.z;
            float y1 = startingPos.y;
            float x2 = arrivingPos.z;
            float y2 = arrivingPos.y;
            float x3 = vertex.z;
            float y3 = vertex.y;

            float denom = (x1 - x2) * (x1 - x3) * (x2 - x3);

            var z_dist = (arrivingPos.z - startingPos.z) / framesNum;
            var x_dist = (arrivingPos.x - startingPos.x) / framesNum;

            float A = (x3 * (y2 - y1) + x2 * (y1 - y3) + x1 * (y3 - y2)) / denom;
            float B = (float)(Math.Pow(x3, 2) * (y1 - y2) + Math.Pow(x2, 2) * (y3 - y1) + Math.Pow(x1, 2) * (y2 - y3)) / denom;
            float C = (x2 * x3 * (x2 - x3) * y1 + x3 * x1 * (x3 - x1) * y2 + x1 * x2 * (x1 - x2) * y3) / denom;

            float newX = startingPos.z;
            float newZ = startingPos.x;

            for (int i = 0; i < framesNum; i++)
            {
                newX += z_dist;
                newZ += x_dist;
                float yToBeFound = A * (newX * newX) + B * newX + C;
                frames[i] = new LtVector3f(newZ, yToBeFound, newX);
            }
            return frames;
        }

    }
}

