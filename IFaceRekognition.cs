using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace TestForms
{
    interface IFaceRekognition
    {
        void initialize();
        void release();
        void requestRekognition(Bitmap bitmap);
        void getRekognitionResult();
        void setOnResultListener();

    }
}
