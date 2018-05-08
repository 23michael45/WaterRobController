using ChartAndGraph;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYPlotMeter : MeterBase<Vector2> {

    protected List<Vector2> _XYPlotValueArr = new List<Vector2>();

    public float _XUnit = 1.0f;
    public float _YUnit = 1.0f;

    public float _YMax = 0f;
    public float _YMin = 20;
    
    public int _DataCount = 10;

    public GraphChart _Graph;

    bool _Dirty = false;

    public string _DataName = "TimeCurrent";
    protected override void Start()
    {
        base.Start();
        

        if (_Graph == null)
        {
            _Graph = gameObject.GetComponentsInChildren<GraphChart>(true)[0];
            
            _Graph.HorizontalValueToStringMap.Add(5, "Sec");
            _Graph.VerticalValueToStringMap.Add(5, "A");
            
        }


    }

    protected override void UpdateValue()
    {
        if(_Dirty)
        {
            _Dirty = false;
            
            _Graph.DataSource.StartBatch();
            


            _Graph.DataSource.ClearCategory(_DataName);

            _Graph.DataSource.AutomaticVerticallView = false;
            _Graph.DataSource.VerticalViewSize = _YMax - _YMin;
            _Graph.DataSource.VerticalViewOrigin = _YMin;

            _Graph.DataSource.AutomaticHorizontalView = false;
            _Graph.DataSource.HorizontalViewSize = _DataCount - 1;
            _Graph.DataSource.HorizontalViewOrigin = 0;

            for (int i = 0; i < _XYPlotValueArr.Count; i++)
            {
                Vector2 data = _XYPlotValueArr[i];
                _Graph.DataSource.AddPointToCategory(_DataName, i,data.y); 
          
            }

            _Graph.DataSource.MakeCurveCategorySmooth(_DataName);
            _Graph.DataSource.EndBatch(); // finally we call EndBatch , this will cause the GraphChart to redraw itself
        
        }
        

    }
    public override void SetCurrentValue(Vector2 v)
    {
        _CurrentValue = v;

        _XYPlotValueArr.Add(v);
        if(_XYPlotValueArr.Count > _DataCount)
        {
            _XYPlotValueArr.Remove(_XYPlotValueArr[0]);
        }

        _Dirty = true;
    }
}
