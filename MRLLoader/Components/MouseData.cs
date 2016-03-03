using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MRL.Commons;
using MRL.Components.Tools.Objects;
using MRL.Components.Tools.Shapes;

namespace MRL.Components
{
    public static class MouseData
    {       
        public static Object SelectedObject;
        public static ShapeBase SelectedShape;

        public static List<Object> objectCollection = new List<Object>();
        public static int TabIndex_Collection = -1;

        public static bool ForceForNoChangeSelectedObject = false;

        public static Size ViewerSize = new Size(500, 400);

        //this method should insert in a global class that all widgets have access to it 
        //public static Matrix transformationMatrix(Point center)
        //{
        //    Matrix m = new Matrix();

        //    float offsetX = (MouseData.DrawPoint.X + MouseData.Offset.X)
        //        , offsetY = (MouseData.DrawPoint.Y + MouseData.Offset.Y);

        //    m.Scale(MouseData.Scale, MouseData.Scale, MatrixOrder.Append);
        //    m.Translate(offsetX, offsetY, MatrixOrder.Append);

        //    return m;
        //}
        //public static bool isIntArea(GraphicsPath gp)
        //{
        //    int sx = -(MouseData.DrawPoint.X + MouseData.Offset.X);
        //    int sy = -(MouseData.DrawPoint.Y + MouseData.Offset.Y);
        //    int ex = sx + ViewerSize.Width;
        //    int ey = sy + ViewerSize.Height;
        //    return gp.GetBounds(MouseData.transformationMatrix(MouseData.Origin)).IntersectsWith(new RectangleF(0, 0, ViewerSize.Width, ViewerSize.Height));
        //}

        public static void addObjectToCollection(Object o)
        {
            lock (((ICollection)objectCollection).SyncRoot)
            {
                objectCollection.Add(o);
            }
        }

        public static void removeObjectFromCollection(Object o)
        {
            lock (((ICollection)objectCollection).SyncRoot)
            {
                objectCollection.Remove(o);
            }
        }

        public static void changeTabIndex(bool next)
        {
            lock (((ICollection)objectCollection).SyncRoot)
            {
                if (objectCollection.Count == 0) return;

                if (next)
                    TabIndex_Collection++;
                else
                    TabIndex_Collection--;

                if (TabIndex_Collection < 0) TabIndex_Collection = objectCollection.Count - 1;
                if (TabIndex_Collection >= objectCollection.Count) TabIndex_Collection = 0;

                Object _selectedObject = objectCollection[TabIndex_Collection];

                if (_selectedObject is AnnotationState)
                    MouseData.SelectedObject = _selectedObject;
                else if (_selectedObject is RobotShape)
                {
                    MouseData.SelectedObject = ((RobotShape)_selectedObject).RobotInfo;
                    MouseData.SelectedShape = ((RobotShape)_selectedObject);
                }
                else if (_selectedObject is VictimShape)
                {
                    VictimShape vShape = (VictimShape)_selectedObject;
                    MouseData.SelectedObject = vShape.VictimInfo;
                    MouseData.SelectedShape = vShape;

                    //Victim v = new Victim(vShape.VictimInfo.ID, vShape.VictimInfo.Name,
                    //                      vShape.VictimInfo.Position, vShape.VictimInfo.Status,
                    //                      0);

                    if (ProjectCommons.imageWidget_OnVictimUpdated != null)
                        ProjectCommons.imageWidget_OnVictimUpdated(vShape);
                }
            }
        }

    }
}
