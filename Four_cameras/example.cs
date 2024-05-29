using XemaCsharp;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;

namespace example
{
    class Program
    {

        public static bool camera_1(string ip,out bool flag, ref byte[] brightness1, ref float[] depth1, ref float[] pointcloud1)
        {

            flag =false;

            //string cameraId = args[1];
            XemaCls xema = new XemaCls();

            //string cameraId = "192.168.3.63";
            int ret_code = -1;
            ret_code = xema.DfConnect_Csharp(ip);

            int width = 0;
            int height = 0;
            int channels = 1;
            if (ret_code == 0)
            {

                ret_code = xema.GetCameraResolution_Csharp(out width, out height);
                Console.WriteLine("width:{0}", width);
                Console.WriteLine("height:{0}", height);
                ret_code = xema.DfGetCameraChannels_Csharp(out channels);
                Console.WriteLine("channels:{0}", channels);
            }
            else
            {
                Console.WriteLine("Connect camera Error!!");
            }

            CalibrationParam calibrationParam;
            ret_code = xema.DfGetCalibrationParam_Csharp(out calibrationParam);
            if (ret_code == 0)
            {
                Console.WriteLine("内参：");
                for (int i = 0; i < calibrationParam.intrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.intrinsic[i]);
                }
                Console.WriteLine("外参：");
                for (int i = 0; i < calibrationParam.extrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.extrinsic[i]);
                }
                Console.WriteLine("畸变：");
                for (int i = 0; i < calibrationParam.distortion.Length; i++)
                {
                    Console.WriteLine(calibrationParam.distortion[i]);
                }
            }
            else
            {
                Console.WriteLine("Get Calibration Data Error!!");
            }

            //分配内存保存采集结果


            StringBuilder timestamp = new StringBuilder(30);

            byte[] brightnessArray = new byte[width * height];
            IntPtr brightnessPtr = Marshal.AllocHGlobal(brightnessArray.Length);



            float[] depthArray = new float[width * height];
            IntPtr depthPtr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float));

            float[] point_cloud_Array = new float[width * height * 3];
            IntPtr point_cloud_Ptr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float) * 3);

            if (ret_code == 0)
            {
                ret_code = xema.DfSetParamSmoothing_Csharp(1);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Smooothing Error!!");
                }
                ret_code = xema.DfSetParamCameraConfidence_Csharp(2);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Confidence Error!!");
                }
                ret_code = xema.DfSetParamCameraGain_Csharp(0);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Gain Error!!");
                }
                ret_code = xema.DfSetParamDepthFilter_Csharp(1, 70);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera DepthFilter Error!!");
                }
                ret_code = xema.DfSetParamRadiusFilter_Csharp(1, 2.5f, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera RadiusFilter Error!!");
                }
                ret_code = xema.DfSetParamReflectFilter_Csharp(1, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera ReflectFilter Error!!");
                }
            }

            //单曝光模式
            if (false)
            {
                ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera LedCurrent Error!!");
                }
                ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera CameraExposure Error!!");
                }
                ret_code = xema.DfCaptureData_Csharp(1, timestamp);
                string timestampString = timestamp.ToString();
                Console.WriteLine("Success！timestamp:" + timestampString);

            }

            else
            {
                if (true)
                {
                    //采集HDR模式
                    int num = 2;
                    int model = 1;
                    int[] exposureParam = { 5000, 8000 };
                    int[] ledParam = { 1023, 1023 };

                    ret_code = xema.DfSetParamMixedHdr_Csharp(num, exposureParam, ledParam);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera HDR Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(model);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;
                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(2, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);

                }
                else
                {
                    //采集重复曝光模式
                    ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera LedCurrent Error!!");
                    }
                    ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera CameraExposure Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;

                    ret_code = xema.DfSetParamRepetitionExposureNum_Csharp(5);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Repetition Exposure Num Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(2);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(5, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);
                }
            }

            if (ret_code == 0)
            {
                //获取亮度图数据
                ret_code = xema.DfGetBrightnessData_Csharp(brightnessPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Brightness!!");
                    Marshal.Copy(brightnessPtr, brightnessArray, 0, brightnessArray.Length);
                    Array.Copy(brightnessArray, brightness1, brightnessArray.Length);
                    //for (int i = 0; i < brightnessArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("brightnessArray[{0}] = {1}", i, brightnessArray[i]);
                    //}
                    Marshal.FreeHGlobal(brightnessPtr);
                }

                //获取深度图数据
                ret_code = xema.DfGetDepthDataFloat_Csharp(depthPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Depth!!");
                    Marshal.Copy(depthPtr, depthArray, 0, depthArray.Length);
                    Array.Copy(depthArray, depth1, depthArray.Length);
                    //for (int i = 0; i < depthArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("deptharray[{0}] = {1}", i, depthArray[i]);
                    //}

                    Marshal.FreeHGlobal(depthPtr);
                }

                //获取点云数据
                ret_code = xema.DfGetPointcloudData_Csharp(point_cloud_Ptr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get PointCloud!!");
                    Marshal.Copy(point_cloud_Ptr, point_cloud_Array, 0, point_cloud_Array.Length);

                    Array.Copy(point_cloud_Array, pointcloud1, point_cloud_Array.Length);
                    

                    Marshal.FreeHGlobal(point_cloud_Ptr);



                }
            }
            else
            {
                Console.WriteLine("Capture Data Error!!");
            }

            ret_code = xema.DfDisconnect_Csharp(ip);
            if (ret_code == 0)
            {


                flag = true;
                Console.WriteLine("Camera Disconnect!!");
                //Console.ReadKey();
            }

            return flag;
        }

        public static bool camera_2(string ip, out bool flag, ref byte[] brightness2, ref float[] depth2, ref float[] pointcloud2)
        {

            flag = false;

            //string cameraId = args[1];
            XemaCls xema = new XemaCls();

            //string cameraId = "192.168.3.63";
            int ret_code = -1;
            ret_code = xema.DfConnect_Csharp(ip);

            int width = 0;
            int height = 0;
            int channels = 1;
            if (ret_code == 0)
            {

                ret_code = xema.GetCameraResolution_Csharp(out width, out height);
                Console.WriteLine("width:{0}", width);
                Console.WriteLine("height:{0}", height);
                ret_code = xema.DfGetCameraChannels_Csharp(out channels);
                Console.WriteLine("channels:{0}", channels);
            }
            else
            {
                Console.WriteLine("Connect camera Error!!");
            }

            CalibrationParam calibrationParam;
            ret_code = xema.DfGetCalibrationParam_Csharp(out calibrationParam);
            if (ret_code == 0)
            {
                Console.WriteLine("内参：");
                for (int i = 0; i < calibrationParam.intrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.intrinsic[i]);
                }
                Console.WriteLine("外参：");
                for (int i = 0; i < calibrationParam.extrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.extrinsic[i]);
                }
                Console.WriteLine("畸变：");
                for (int i = 0; i < calibrationParam.distortion.Length; i++)
                {
                    Console.WriteLine(calibrationParam.distortion[i]);
                }
            }
            else
            {
                Console.WriteLine("Get Calibration Data Error!!");
            }

            //分配内存保存采集结果


            StringBuilder timestamp = new StringBuilder(30);

            byte[] brightnessArray = new byte[width * height];
            IntPtr brightnessPtr = Marshal.AllocHGlobal(brightnessArray.Length);



            float[] depthArray = new float[width * height];
            IntPtr depthPtr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float));

            float[] point_cloud_Array = new float[width * height * 3];
            IntPtr point_cloud_Ptr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float) * 3);

            if (ret_code == 0)
            {
                ret_code = xema.DfSetParamSmoothing_Csharp(1);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Smooothing Error!!");
                }
                ret_code = xema.DfSetParamCameraConfidence_Csharp(2);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Confidence Error!!");
                }
                ret_code = xema.DfSetParamCameraGain_Csharp(0);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Gain Error!!");
                }
                ret_code = xema.DfSetParamDepthFilter_Csharp(1, 70);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera DepthFilter Error!!");
                }
                ret_code = xema.DfSetParamRadiusFilter_Csharp(1, 2.5f, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera RadiusFilter Error!!");
                }
                ret_code = xema.DfSetParamReflectFilter_Csharp(1, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera ReflectFilter Error!!");
                }
            }

            //单曝光模式
            if (false)
            {
                ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera LedCurrent Error!!");
                }
                ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera CameraExposure Error!!");
                }
                ret_code = xema.DfCaptureData_Csharp(1, timestamp);
                string timestampString = timestamp.ToString();
                Console.WriteLine("Success！timestamp:" + timestampString);

            }

            else
            {
                if (true)
                {
                    //采集HDR模式
                    int num = 2;
                    int model = 1;
                    int[] exposureParam = { 5000, 8000 };
                    int[] ledParam = { 1023, 1023 };

                    ret_code = xema.DfSetParamMixedHdr_Csharp(num, exposureParam, ledParam);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera HDR Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(model);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;
                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(2, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);

                }
                else
                {
                    //采集重复曝光模式
                    ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera LedCurrent Error!!");
                    }
                    ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera CameraExposure Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;

                    ret_code = xema.DfSetParamRepetitionExposureNum_Csharp(5);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Repetition Exposure Num Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(2);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(5, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);
                }
            }

            if (ret_code == 0)
            {
                //获取亮度图数据
                ret_code = xema.DfGetBrightnessData_Csharp(brightnessPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Brightness!!");
                    Marshal.Copy(brightnessPtr, brightnessArray, 0, brightnessArray.Length);

                    Array.Copy(brightnessArray, brightness2, brightnessArray.Length);
                    //for (int i = 0; i < brightnessArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("brightnessArray[{0}] = {1}", i, brightnessArray[i]);
                    //}
                    Marshal.FreeHGlobal(brightnessPtr);
                }

                //获取深度图数据
                ret_code = xema.DfGetDepthDataFloat_Csharp(depthPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Depth!!");
                    Marshal.Copy(depthPtr, depthArray, 0, depthArray.Length);
                    Array.Copy(depthArray, depth2, depthArray.Length);
                    //for (int i = 0; i < depthArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("deptharray[{0}] = {1}", i, depthArray[i]);
                    //}

                    Marshal.FreeHGlobal(depthPtr);
                }

                //获取点云数据
                ret_code = xema.DfGetPointcloudData_Csharp(point_cloud_Ptr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get PointCloud!!");
                    Marshal.Copy(point_cloud_Ptr, point_cloud_Array, 0, point_cloud_Array.Length);

                    Array.Copy(point_cloud_Array, pointcloud2, point_cloud_Array.Length);

                    Marshal.FreeHGlobal(point_cloud_Ptr);
                }
            }
            else
            {
                Console.WriteLine("Capture Data Error!!");
            }

            ret_code = xema.DfDisconnect_Csharp(ip);
            if (ret_code == 0)
            {


                flag = true;
                Console.WriteLine("Camera Disconnect!!");
                //Console.ReadKey();
            }

            return flag;
        }


        public static bool camera_3(string ip, out bool flag, ref byte[] brightness3, ref float[] depth3, ref float[] pointcloud3)
        {

            flag = false;

            //string cameraId = args[1];
            XemaCls xema = new XemaCls();

            //string cameraId = "192.168.3.63";
            int ret_code = -1;
            ret_code = xema.DfConnect_Csharp(ip);

            int width = 0;
            int height = 0;
            int channels = 1;
            if (ret_code == 0)
            {

                ret_code = xema.GetCameraResolution_Csharp(out width, out height);
                Console.WriteLine("width:{0}", width);
                Console.WriteLine("height:{0}", height);
                ret_code = xema.DfGetCameraChannels_Csharp(out channels);
                Console.WriteLine("channels:{0}", channels);
            }
            else
            {
                Console.WriteLine("Connect camera Error!!");
            }

            CalibrationParam calibrationParam;
            ret_code = xema.DfGetCalibrationParam_Csharp(out calibrationParam);
            if (ret_code == 0)
            {
                Console.WriteLine("内参：");
                for (int i = 0; i < calibrationParam.intrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.intrinsic[i]);
                }
                Console.WriteLine("外参：");
                for (int i = 0; i < calibrationParam.extrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.extrinsic[i]);
                }
                Console.WriteLine("畸变：");
                for (int i = 0; i < calibrationParam.distortion.Length; i++)
                {
                    Console.WriteLine(calibrationParam.distortion[i]);
                }
            }
            else
            {
                Console.WriteLine("Get Calibration Data Error!!");
            }

            //分配内存保存采集结果


            StringBuilder timestamp = new StringBuilder(30);

            byte[] brightnessArray = new byte[width * height];
            IntPtr brightnessPtr = Marshal.AllocHGlobal(brightnessArray.Length);



            float[] depthArray = new float[width * height];
            IntPtr depthPtr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float));

            float[] point_cloud_Array = new float[width * height * 3];
            IntPtr point_cloud_Ptr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float) * 3);

            if (ret_code == 0)
            {
                ret_code = xema.DfSetParamSmoothing_Csharp(1);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Smooothing Error!!");
                }
                ret_code = xema.DfSetParamCameraConfidence_Csharp(2);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Confidence Error!!");
                }
                ret_code = xema.DfSetParamCameraGain_Csharp(0);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Gain Error!!");
                }
                ret_code = xema.DfSetParamDepthFilter_Csharp(1, 70);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera DepthFilter Error!!");
                }
                ret_code = xema.DfSetParamRadiusFilter_Csharp(1, 2.5f, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera RadiusFilter Error!!");
                }
                ret_code = xema.DfSetParamReflectFilter_Csharp(1, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera ReflectFilter Error!!");
                }
            }

            //单曝光模式
            if (false)
            {
                ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera LedCurrent Error!!");
                }
                ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera CameraExposure Error!!");
                }
                ret_code = xema.DfCaptureData_Csharp(1, timestamp);
                string timestampString = timestamp.ToString();
                Console.WriteLine("Success！timestamp:" + timestampString);

            }

            else
            {
                if (true)
                {
                    //采集HDR模式
                    int num = 2;
                    int model = 1;
                    int[] exposureParam = { 5000, 8000 };
                    int[] ledParam = { 1023, 1023 };

                    ret_code = xema.DfSetParamMixedHdr_Csharp(num, exposureParam, ledParam);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera HDR Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(model);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;
                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(2, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);

                }
                else
                {
                    //采集重复曝光模式
                    ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera LedCurrent Error!!");
                    }
                    ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera CameraExposure Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;

                    ret_code = xema.DfSetParamRepetitionExposureNum_Csharp(5);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Repetition Exposure Num Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(2);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(5, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);
                }
            }

            if (ret_code == 0)
            {
                //获取亮度图数据
                ret_code = xema.DfGetBrightnessData_Csharp(brightnessPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Brightness!!");
                    Marshal.Copy(brightnessPtr, brightnessArray, 0, brightnessArray.Length);
                    Array.Copy(brightnessArray, brightness3, brightnessArray.Length);
                    //for (int i = 0; i < brightnessArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("brightnessArray[{0}] = {1}", i, brightnessArray[i]);
                    //}
                    Marshal.FreeHGlobal(brightnessPtr);
                }

                //获取深度图数据
                ret_code = xema.DfGetDepthDataFloat_Csharp(depthPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Depth!!");
                    Marshal.Copy(depthPtr, depthArray, 0, depthArray.Length);
                    Array.Copy(depthArray, depth3, depthArray.Length);
                    //for (int i = 0; i < depthArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("deptharray[{0}] = {1}", i, depthArray[i]);
                    //}

                    Marshal.FreeHGlobal(depthPtr);
                }

                //获取点云数据
                ret_code = xema.DfGetPointcloudData_Csharp(point_cloud_Ptr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get PointCloud!!");
                    Marshal.Copy(point_cloud_Ptr, point_cloud_Array, 0, point_cloud_Array.Length);
                    Array.Copy(point_cloud_Array, pointcloud3, point_cloud_Array.Length);


                    Marshal.FreeHGlobal(point_cloud_Ptr);



                }
            }
            else
            {
                Console.WriteLine("Capture Data Error!!");
            }

            ret_code = xema.DfDisconnect_Csharp(ip);
            if (ret_code == 0)
            {


                flag = true;
                Console.WriteLine("Camera Disconnect!!");
                //Console.ReadKey();
            }

            return flag;
        }

        public static bool camera_4(string ip, out bool flag, ref byte[] brightness4, ref float[] depth4, ref float[] pointcloud4)
        {

            flag = false;

            //string cameraId = args[1];
            XemaCls xema = new XemaCls();

            //string cameraId = "192.168.3.63";
            int ret_code = -1;
            ret_code = xema.DfConnect_Csharp(ip);

            int width = 0;
            int height = 0;
            int channels = 1;
            if (ret_code == 0)
            {

                ret_code = xema.GetCameraResolution_Csharp(out width, out height);
                Console.WriteLine("width:{0}", width);
                Console.WriteLine("height:{0}", height);
                ret_code = xema.DfGetCameraChannels_Csharp(out channels);
                Console.WriteLine("channels:{0}", channels);
            }
            else
            {
                Console.WriteLine("Connect camera Error!!");
            }

            CalibrationParam calibrationParam;
            ret_code = xema.DfGetCalibrationParam_Csharp(out calibrationParam);
            if (ret_code == 0)
            {
                Console.WriteLine("内参：");
                for (int i = 0; i < calibrationParam.intrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.intrinsic[i]);
                }
                Console.WriteLine("外参：");
                for (int i = 0; i < calibrationParam.extrinsic.Length; i++)
                {
                    Console.WriteLine(calibrationParam.extrinsic[i]);
                }
                Console.WriteLine("畸变：");
                for (int i = 0; i < calibrationParam.distortion.Length; i++)
                {
                    Console.WriteLine(calibrationParam.distortion[i]);
                }
            }
            else
            {
                Console.WriteLine("Get Calibration Data Error!!");
            }

            //分配内存保存采集结果


            StringBuilder timestamp = new StringBuilder(30);

            byte[] brightnessArray = new byte[width * height];
            IntPtr brightnessPtr = Marshal.AllocHGlobal(brightnessArray.Length);



            float[] depthArray = new float[width * height];
            IntPtr depthPtr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float));

            float[] point_cloud_Array = new float[width * height * 3];
            IntPtr point_cloud_Ptr = Marshal.AllocHGlobal(depthArray.Length * sizeof(float) * 3);

            if (ret_code == 0)
            {
                ret_code = xema.DfSetParamSmoothing_Csharp(1);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Smooothing Error!!");
                }
                ret_code = xema.DfSetParamCameraConfidence_Csharp(2);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Confidence Error!!");
                }
                ret_code = xema.DfSetParamCameraGain_Csharp(0);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera Gain Error!!");
                }
                ret_code = xema.DfSetParamDepthFilter_Csharp(1, 70);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera DepthFilter Error!!");
                }
                ret_code = xema.DfSetParamRadiusFilter_Csharp(1, 2.5f, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera RadiusFilter Error!!");
                }
                ret_code = xema.DfSetParamReflectFilter_Csharp(1, 20);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera ReflectFilter Error!!");
                }
            }

            //单曝光模式
            if (false)
            {
                ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera LedCurrent Error!!");
                }
                ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                if (ret_code != 0)
                {
                    Console.WriteLine("Set Camera CameraExposure Error!!");
                }
                ret_code = xema.DfCaptureData_Csharp(1, timestamp);
                string timestampString = timestamp.ToString();
                Console.WriteLine("Success！timestamp:" + timestampString);

            }

            else
            {
                if (true)
                {
                    //采集HDR模式
                    int num = 2;
                    int model = 1;
                    int[] exposureParam = { 5000, 8000 };
                    int[] ledParam = { 1023, 1023 };

                    ret_code = xema.DfSetParamMixedHdr_Csharp(num, exposureParam, ledParam);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera HDR Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(model);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;
                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(2, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);

                }
                else
                {
                    //采集重复曝光模式
                    ret_code = xema.DfSetParamLedCurrent_Csharp(1023);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera LedCurrent Error!!");
                    }
                    ret_code = xema.DfSetParamCameraExposure_Csharp(20000);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera CameraExposure Error!!");
                    }

                    XemaEngine engine = XemaEngine.Reflect;

                    ret_code = xema.DfSetParamRepetitionExposureNum_Csharp(5);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Repetition Exposure Num Error!!");
                    }

                    ret_code = xema.DfSetParamMultipleExposureModel_Csharp(2);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Multiple Exposure Model Error!!");
                    }

                    ret_code = xema.DfSetCaptureEngine_Csharp(engine);
                    if (ret_code != 0)
                    {
                        Console.WriteLine("Set Camera Capture Engine Error!!");
                    }

                    ret_code = xema.DfCaptureData_Csharp(5, timestamp);
                    string timestampString = timestamp.ToString();
                    Console.WriteLine("Success！timestamp:" + timestampString);
                }
            }

            if (ret_code == 0)
            {
                //获取亮度图数据
                ret_code = xema.DfGetBrightnessData_Csharp(brightnessPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Brightness!!");
                    Marshal.Copy(brightnessPtr, brightnessArray, 0, brightnessArray.Length);
                    Array.Copy(brightnessArray, brightness4, brightnessArray.Length);
                    //for (int i = 0; i < brightnessArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("brightnessArray[{0}] = {1}", i, brightnessArray[i]);
                    //}
                    Marshal.FreeHGlobal(brightnessPtr);
                }

                //获取深度图数据
                ret_code = xema.DfGetDepthDataFloat_Csharp(depthPtr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get Depth!!");
                    Marshal.Copy(depthPtr, depthArray, 0, depthArray.Length);

                    Array.Copy(depthArray, depth4, depthArray.Length);

                    //for (int i = 0; i < depthArray.Length/10; i++)
                    //{
                    //    Console.WriteLine("deptharray[{0}] = {1}", i, depthArray[i]);
                    //}

                    Marshal.FreeHGlobal(depthPtr);
                }

                //获取点云数据
                ret_code = xema.DfGetPointcloudData_Csharp(point_cloud_Ptr);
                if (ret_code == 0)
                {
                    Console.WriteLine("Get PointCloud!!");
                    Marshal.Copy(point_cloud_Ptr, point_cloud_Array, 0, point_cloud_Array.Length);

                    Array.Copy(point_cloud_Array, pointcloud4, point_cloud_Array.Length);


                    Marshal.FreeHGlobal(point_cloud_Ptr);



                }
            }
            else
            {
                Console.WriteLine("Capture Data Error!!");
            }

            ret_code = xema.DfDisconnect_Csharp(ip);
            if (ret_code == 0)
            {


                flag = true;
                Console.WriteLine("Camera Disconnect!!");
                //Console.ReadKey();
            }

            return flag;
        }

        public static void SaveXyzFile(string filename, float[] data, int width, int height)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(filename))
                {
                    for (int i = 0; i < width * height; ++i)
                    {
                      
                        file.WriteLine($"{data[i * 3]} {data[i * 3 + 1]} {data[i * 3 + 2]}");
                    }
                }
            }
            catch (Exception ex)
            {      
                throw new Exception("Failed to open or write to file.", ex);
            }
        }



        static void Main(string[] args)
        {
            while (true)
            {

                //修改相机分辨率
                int width = 1920;
                int height = 1200;

                //开辟内存
                float[] pointcloud1 = new float[width * height * 3];
                float[] pointcloud2 = new float[width * height * 3];
                float[] pointcloud3 = new float[width * height * 3];
                float[] pointcloud4 = new float[width * height * 3];

                byte[] brightness1 = new byte[width * height];
                byte[] brightness2 = new byte[width * height];
                byte[] brightness3 = new byte[width * height];
                byte[] brightness4 = new byte[width * height];

                float[] depth1 = new float[width * height];
                float[] depth2 = new float[width * height];
                float[] depth3 = new float[width * height];
                float[] depth4 = new float[width * height];

                //四台相机ip
                string ip1 = "192.168.3.63";
                string ip2 = "192.168.3.97";
                string ip3 = "192.168.3.63";
                string ip4 = "192.168.3.97";


                bool flag1 = camera_1(ip1, out bool flag11, ref brightness1, ref depth1, ref pointcloud1);
                Console.WriteLine("flag1");
                Console.WriteLine(flag1);

            
                //string filename = "path_to_your_file.xyz"; 
                //SaveXyzFile(filename, pointcloud1, width, height);
                //Console.WriteLine(flag1);

                if (flag1 == true)
                {
                    
                    bool flag2 = camera_2(ip2, out bool flag22, ref brightness2, ref depth2, ref pointcloud2);
                    Console.WriteLine("flag2");
                    Console.WriteLine(flag2);

                    if (flag2 == true)
                    {
                        
                        bool flag3 = camera_3(ip3, out bool flag33, ref brightness3, ref depth3, ref pointcloud3);
                        Console.WriteLine("flag3");
                        Console.WriteLine(flag3);

                        if (flag3 == true)
                        {
                            
                            bool flag4 = camera_4(ip4, out bool flag44, ref brightness4, ref depth4, ref pointcloud4);
                            Console.WriteLine("flag4");
                            Console.WriteLine(flag4);
                        }

                    }


                }
                //Console.ReadKey();
                //return;
            }

        }
    
        
    }
}