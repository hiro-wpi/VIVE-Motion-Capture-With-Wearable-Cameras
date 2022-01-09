using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using UnityEditor;

namespace ExperimentDataRecord {
    public class DataRecordUtil : MonoBehaviour
    {

        /// <summary>
        /// Write to Excel ; Needs OfficeOpenXml.dll;
        /// </summary>
        /// <param name="excelName">excelName</param>
        /// <param name="sheetName">sheetName</param>
        public static void WriteExcel(string raw, string col, string content, string excelName = "DataDir/demo.xlsx", string sheetName = "sheet1")
        {
            // Set Excel FilePath through Panel
            // string outputDir = EditorUtility.SaveFilePanel("Save Excel", "", "New Resource", "xlsx");

            // Define Excel FilePath
            string path = Application.dataPath + "/" + excelName;

            // If directory name doesn't exist, create one
            string dirPath = System.IO.Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Reference to Excel File with path, respawn one if file does not exist
            FileInfo newFile = new FileInfo(path);

            // Open Excel File
            using (ExcelPackage exlpackage = new ExcelPackage(newFile))
            {
                // Choose Excel WorkSheet In Excel File
                ExcelWorksheet worksheet = null;

                if(SheetRespawn(exlpackage, sheetName, out worksheet))
                {
                    Debug.Log("Successfully add the sheet " + sheetName + " to the excel file");
                }
                else
                {
                    Debug.Log(sheetName + " already exists, use the origin one");
                }

                worksheet.Cells[int.Parse(raw), int.Parse(col)].Value = content;
                
                // Add column
                /*worksheet.Cells[2, 1].Value = "ID";
                worksheet.Cells[2, 2].Value = "Product";
                worksheet.Cells[2, 3].Value = "Quantity";
                worksheet.Cells[2, 4].Value = "Price";
                worksheet.Cells[2, 5].Value = "Value";

                // Add raw
                worksheet.Cells["A2"].Value = 12001;
                worksheet.Cells["B2"].Value = "Nails";
                worksheet.Cells["C2"].Value = 37;
                worksheet.Cells["D2"].Value = 3.99;*/

                // Save Excel File
                exlpackage.Save();
            }
        }

        // Get the Excel Worksheet
        public static bool SheetRespawn(ExcelPackage exlpackage, string sheetName, out ExcelWorksheet worksheet)
        {
            for (int i = 1; i <= exlpackage.Workbook.Worksheets.Count; i++)
            {
                if (exlpackage.Workbook.Worksheets[i].ToString().ToLower() == sheetName.ToLower())
                {
                    worksheet = exlpackage.Workbook.Worksheets[i];
                    return false;
                }
            }
            worksheet = exlpackage.Workbook.Worksheets.Add(sheetName);
            return true;
        }

        // Delete the file with path given
        public static bool DeleteFile(string path)
        {
            path = Application.dataPath + '/' + path;
            FileInfo newFile = new FileInfo(path);
            if(newFile.Exists)
            {
                newFile.Delete();
                //FileUtil.DeleteFileOrDirectory(path);
                //AssetDatabase.Refresh();
                Debug.Log("Successfully delete the file: " + path);
                return true;
            }
            Debug.Log("File not found in this path: " + path);
            return false;
        }

        // Delete the Directory with path given
        public static bool DeleteDirectory(string target_dir)
        {
            target_dir = Application.dataPath + '/' + target_dir;
            
            if (Directory.Exists(target_dir))
            {
                string[] files = Directory.GetFiles(target_dir);
                string[] dirs = Directory.GetDirectories(target_dir);

                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in dirs)
                {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target_dir, false);
                Debug.Log("Successfully delete the directory " + target_dir);
                return true;
            }
            else
            {
                Debug.Log("Filepath: " + target_dir + " is not found");
                return false;
            }
        }


        // Take the screenshot
        public static void TakeScreenshot(string dirPath, string filedemoName, int cnt)
        {
            dirPath = Application.dataPath + "/" + dirPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            // Take Screenshot
            ScreenCapture.CaptureScreenshot(dirPath + "/" + filedemoName + "_" + cnt.ToString() + '_' + Time.frameCount.ToString() + ".jpg");

        }

        /// <summary>
        /// 对相机截图。 
        /// </summary>
        /// <returns>The screenshot2.</returns>
        /// <param name="camera">Camera.要被截屏的相机</param>
        /// <param name="rect">Rect.截屏的区域</param>
        public static Texture2D CaptureCamera(Camera camera, Rect rect)
        {
            // 创建一个RenderTexture对象
            RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
            // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
            camera.targetTexture = rt;
            camera.Render();
            //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
            //ps: camera2.targetTexture = rt;
            //ps: camera2.Render();
            //ps: -------------------------------------------------------------------

            // 激活这个rt, 并从中中读取像素。
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素
            screenShot.Apply();

            // 重置相关参数，以使用camera继续在屏幕上显示
            camera.targetTexture = null;
            //ps: camera2.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            GameObject.Destroy(rt);
            // 最后将这些纹理数据，成一个png图片文件
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = Application.dataPath + "/ScreenShot/snapshot.png";
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("截屏了一张照片: {0}", filename));

            return screenShot;
        }
    }

    
}

