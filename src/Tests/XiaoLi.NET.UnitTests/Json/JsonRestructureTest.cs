using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XiaoLi.NET.Mvc.Exceptions;
using Xunit.Abstractions;

namespace XiaoLi.NET.UnitTests.Json;

public class JsonRestructureTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public JsonRestructureTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    void Core()
    {
        _testOutputHelper.WriteLine(Thread.CurrentThread.CurrentCulture.ToString());
        _testOutputHelper.WriteLine(DateTime.Now.ToString("yyyy-M-d dddd"));
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
        _testOutputHelper.WriteLine(Thread.CurrentThread.CurrentCulture.ToString());
        _testOutputHelper.WriteLine(DateTime.Now.ToString("yyyy-M-d dddd"));
        // [][] -> [][]
        // [属性,属性,属性] -》1：1
        // len  危险

        //  a[].b[].c  ->  x[].y.z[].d
        //  a.c        ->  x.y[].z[].d
        //  order_weight ->  boxList[].weight[].x.c   NULL
        //  a[].c      ->  x.y[].z[].d
        var str = //"[{\"orderNo\":1},{\"orderNo\":2},{\"orderNo\":3}]";
        """
{
	"x": [{
		"order_weight": 1
	}, {
		"order_weight": 2
	}, {
		"order_weight": 3
	}],
	"C": [{
		"a": 1
	}, {
		"a": 2
	}, {
		"a": 3
	}],
	"B": {
		"a": 2,
		"b": [{
			"a": 101,
			"b": "b1",
			"c": [123]
		}, {
			"a": 102,
			"b": "b2",
			"c": [2, 3, 7, 9]
		}, {
			"a": 103,
			"b": "b3",
			"c": [2]
		}]
	},

}
""";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps();

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList));

        _testOutputHelper.WriteLine(res);
    }

    [Fact]
    void ss()
    {
        string json = """
                      {"1ZB323Y27919897186":{"detail":[{"time":"2023-10-23 11:23:55","content":"CREATE ORDER","code":"SH101","status_uid":"9d6e55337347dc0d62263ad6618f5be9155579"},{"time":"2023-10-23 14:40:00","content":"Wolka Kosowska,PL,Pickup Scan ","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 14:40:00","content":"Wolka Kosowska,PL,Pickup Scan ","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 18:37:09","content":"Wolka Kosowska,PL,Origin Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 18:37:09","content":"Wolka Kosowska,PL,Origin Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 20:03:52","content":"Hoofddorp,NL,Value of Goods charges are due for this package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 20:03:52","content":"Hoofddorp,NL,Value of Goods charges are due for this package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 21:47:00","content":"Wolka Kosowska,PL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 21:47:00","content":"Wolka Kosowska,PL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 22:59:59","content":"已收件","code":"SH115","status_uid":"14e23a99840836c158ff5fc6c2b3bcf8510298"},{"time":"2023-10-23 23:15:00","content":"Dobra Strykow,PL,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 23:15:00","content":"Dobra Strykow,PL,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-23 23:59:00","content":"已发件","code":"SH103","status_uid":"54e7252ba81df0ea10ef5a122655c35c502210"},{"time":"2023-10-24 10:58:00","content":"Langenhagen,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 10:58:00","content":"Langenhagen,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 20:04:00","content":"Langenhagen,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 20:04:00","content":"Langenhagen,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 23:23:00","content":"Herne-Boernig,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 23:23:00","content":"Herne-Boernig,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 03:47:00","content":"Herne-Boernig,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 03:47:00","content":"Herne-Boernig,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 06:45:00","content":"Hoofddorp,NL,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 06:45:00","content":"Hoofddorp,NL,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 07:58:19","content":"Hoofddorp,NL,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 07:58:19","content":"Hoofddorp,NL,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 07:58:20","content":"Hoofddorp,NL,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 07:58:20","content":"Hoofddorp,NL,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 07:58:20","content":"Hoofddorp,NL,Out For Delivery","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 10:37:03","content":"Hoofddorp,NL,Out For Delivery Today","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 10:37:03","content":"Hoofddorp,NL,Out For Delivery Today","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 13:32:56","content":"Hoofddorp,NL,Customer was not available when UPS attempted delivery. Will deliver to a nearby UPS Access Point™ for customer pick up.","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 13:32:56","content":"Hoofddorp,NL,Customer was not available when UPS attempted delivery. Will deliver to a nearby UPS Access Point™ for customer pick up.","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 16:06:44","content":"Hoofddorp,NL,Delivered to UPS Access Point™ ","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 16:06:44","content":"Hoofddorp,NL,Delivered to UPS Access Point™ ","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 16:09:06","content":"UPS Access Point™ possession ","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 16:09:06","content":"Hoofddorp,NL,UPS Access Point™ possession ","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-25 16:09:06","content":"Hoofddorp,NL,UPS Access Point™ possession ","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-31 10:15:43","content":"Hoofddorp,NL,The package remains at the UPS Access Point™ location and is reaching the maximum days allowed to be held.","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-31 10:15:43","content":"Hoofddorp,NL,The package remains at the UPS Access Point™ location and is reaching the maximum days allowed to be held.","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-10-31 10:15:43","content":"The package remains at the UPS Access Point™ location and is reaching the maximum days allowed to be held.","code":"SH146","status_uid":"e73761fe06c044e35536d072cb91dc15165015"},{"time":"2023-11-11 10:51:22","content":"Hoofddorp,NL,The maximum days to hold the package at the UPS Access Point™ location expired. The package will be returned.","code":"SH123","status_uid":"1f45d5f7f6ff662f6a01653716853872105308"},{"time":"2023-11-11 10:51:22","content":"The maximum days to hold the package at the UPS Access Point™ location expired. The package will be returned.","code":"SH123","status_uid":"1f45d5f7f6ff662f6a01653716853872105308"},{"time":"2023-11-11 10:51:22","content":"Hoofddorp,NL,The maximum days to hold the package at the UPS Access Point™ location expired. The package will be returned.","code":"SH123","status_uid":"1f45d5f7f6ff662f6a01653716853872105308"},{"time":"2023-12-13 04:46:06","content":"We've begun an investigation to locate the package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-12-13 04:46:06","content":"We've begun an investigation to locate the package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"}],"pod":{"code":"SH110","status":"PROCESSING","status_uid":"85321a34861a199683e30c24c0731ddb987155","detail":"Weve begun an investigation to locate the package.","time":"2023-12-13 04:46:06"},"number":{"track_bill_no":"1ZB323Y27919897186","bill_no":"1ZB323Y27919897186","order_no":"RN231020112542146542","actual_weight":6,"original_bill_no":"","trans_bill_no":"","return_bill_no":"","length":"1","width":"1","height":"1","cbm":"0.0001"}},"1ZB323Y27913812643":{"detail":[{"time":"2023-10-24 12:02:57","content":"CREATE ORDER","code":"SH101","status_uid":"9d6e55337347dc0d62263ad6618f5be9155579"},{"time":"2023-10-24 19:54:46","content":"Wolka Kosowska,PL,Origin Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 19:54:46","content":"Wolka Kosowska,PL,Origin Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 22:13:00","content":"Wolka Kosowska,PL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 22:13:00","content":"Wolka Kosowska,PL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-24 22:59:59","content":"已收件","code":"SH115","status_uid":"14e23a99840836c158ff5fc6c2b3bcf8510298"},{"time":"2023-10-24 23:59:00","content":"已发件","code":"SH103","status_uid":"54e7252ba81df0ea10ef5a122655c35c502210"},{"time":"2023-10-25 05:18:00","content":"Biruliskiai,LT,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 05:18:00","content":"Biruliskiai,LT,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 05:19:00","content":"Biruliskiai,LT,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 05:19:00","content":"Biruliskiai,LT,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 19:43:00","content":"Biruliskiai,LT,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 19:43:00","content":"Biruliskiai,LT,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 21:04:00","content":"Riga,LV,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 21:04:00","content":"Riga,LV,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 23:33:00","content":"Riga,LV,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-25 23:33:00","content":"Riga,LV,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-26 05:30:00","content":"Tallinn,EE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-26 05:30:00","content":"Tallinn,EE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-26 18:30:00","content":"Tallinn,EE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-26 18:30:00","content":"Tallinn,EE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 01:00:00","content":"Helsinki,FI,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 01:00:00","content":"Helsinki,FI,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 02:58:01","content":"Helsinki,FI,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 02:58:01","content":"Helsinki,FI,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 08:00:31","content":"Helsinki,FI,The package has been rescheduled for a future delivery date.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 08:00:31","content":"Helsinki,FI,The package has been rescheduled for a future delivery date.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 08:30:28","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-27 08:30:28","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-30 07:54:47","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-30 07:54:47","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-31 08:37:57","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-10-31 08:37:57","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-01 08:23:10","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-01 08:23:10","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-02 09:40:06","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-02 09:40:06","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-03 05:44:27","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-03 05:44:27","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-06 10:19:44","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-06 10:19:44","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-07 10:51:04","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-07 10:51:04","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-08 08:31:39","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-08 08:31:39","content":"Helsinki,FI,Warehouse Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-10 06:54:14","content":"Helsinki,FI,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-10 06:54:14","content":"Helsinki,FI,Import Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-12-14 03:57:12","content":"We've begun an investigation to locate the package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-12-14 03:57:12","content":"We've begun an investigation to locate the package.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2024-01-04 01:55:50","content":"Your investigation has been closed with a proof of delivery.","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2024-01-20 10:01:47","content":"RETURNING","code":"SH123","status_uid":"1f45d5f7f6ff662f6a01653716853872105308"}],"pod":{"code":"SH123","status":"RETURNING","status_uid":"1f45d5f7f6ff662f6a01653716853872105308","detail":"","time":"2024-01-20 10:01:47"},"number":{"track_bill_no":"1ZB323Y27913812643","bill_no":"1ZB323Y27913812643","order_no":"RN231023142732150705","actual_weight":6.5,"original_bill_no":"","trans_bill_no":"","return_bill_no":"1ZB323Y27919205539","length":"1","width":"1","height":"1","cbm":"0.0001"}},"1ZB323Y16807510691":{"detail":[{"time":"2023-11-17 17:09:42","content":"CREATE ORDER","code":"SH101","status_uid":"9d6e55337347dc0d62263ad6618f5be9155579"},{"time":"2023-11-20 16:30:11","content":"Wolka Kosowska,PL,Pickup Scan ","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-20 20:36:45","content":"Wolka Kosowska,PL,Origin Scan","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-20 22:59:59","content":"已收件","code":"SH115","status_uid":"14e23a99840836c158ff5fc6c2b3bcf8510298"},{"time":"2023-11-21 00:45:00","content":"Wolka Kosowska,PL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-21 10:52:02","content":"A delivery change for this package is in progress. / The package will be returned to the sender.","code":"SH112","status_uid":"9d97b15e661204ab853b1ba7c476f5c3472601"},{"time":"2023-11-21 19:04:00","content":"Langenhagen,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-21 21:00:00","content":"Uncontrollable events have delayed delivery. We are adjusting delivery plans as quickly as possible. Please check back on the next business day for updates.","code":"SH112","status_uid":"9d97b15e661204ab853b1ba7c476f5c3472601"},{"time":"2023-11-22 20:52:00","content":"Langenhagen,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-22 23:54:00","content":"Herne-Boernig,DE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-23 01:31:00","content":"Herne-Boernig,DE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-23 04:45:00","content":"Brussels,BE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-23 11:03:00","content":"Brussels,BE,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-23 12:45:00","content":"Eindhoven,NL,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-24 02:32:00","content":"Eindhoven,NL,Departed from Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-24 04:15:00","content":"Brussels,BE,Arrived at Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-24 07:02:00","content":"Brussels,BE,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-24 10:34:05","content":"Brussels,BE,The receiver has canceled the product order and refused delivery.","code":"SH105","status_uid":"4fbaa621732e4b174f3ed97e2311e290963492"},{"time":"2023-11-24 10:34:08","content":"Brussels,BE,Processing at UPS Facility","code":"SH110","status_uid":"85321a34861a199683e30c24c0731ddb987155"},{"time":"2023-11-27 05:25:53","content":"Brussels,BE,We're returning this package to the sender as requested by the receiver.","code":"SH123","status_uid":"1f45d5f7f6ff662f6a01653716853872105308"},{"time":"2023-12-08 00:19:56","content":"上架][RTS]16050202","code":"SH124","status_uid":"6b8be55431aa993abe7ea60c4b6fdd5e587482"}],"pod":{"code":"SH124","status":"RTS","status_uid":"6b8be55431aa993abe7ea60c4b6fdd5e587482","detail":"上架][RTS]16050202","time":"2023-12-08 00:19:56"},"number":{"track_bill_no":"1ZB323Y16807510691","bill_no":"1ZB323Y16807510691","order_no":"381344-00026388","actual_weight":0.5,"original_bill_no":"","trans_bill_no":"","return_bill_no":"1ZB323Y16820326428","length":"1","width":"1","height":"1","cbm":"0.0001"}}}
                      """;

        var res = JToken.Parse(json);

        if (res is JObject jsonObject)
        {
            var arr = new JArray();
            foreach (var item in jsonObject)
            {
                arr.Add(item.Value);
            }

            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(arr));
        }
    }
    
    
    [Fact]
    void Jarray()
    {
        // [{}] -> []
        // [{}] -> {[]}
        // [{}] -> [{}]
        var str = "{\"orderNo\":1}";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps2();
        if (jtoken is JArray)
        {
            list.ForEach(x =>
            {
                if (!string.IsNullOrEmpty(x.MapAlias))
                {
                    x.MapAlias = "[*]." + x.MapAlias;
                }
            });
        }

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList, MappedJsonType.Array));

        _testOutputHelper.WriteLine(res);
    }
    
    [Fact]
    void Value()
    {
        // {} -> v
        var str = "1";
        var jtoken = JToken.Parse(str);
        //_testOutputHelper.WriteLine(jtoken.SelectToken("[0].orderNo").ToString());
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }

        _testOutputHelper.WriteLine("==============");

        List<Parameter> list = GetMaps3();

        List<ParameterNode> treeList = BuildTreeList(list, 0);

        var res = JsonConvert.SerializeObject(dfs_json(jtoken, treeList, MappedJsonType.Value));

        _testOutputHelper.WriteLine(res);
    }

    [Fact]
    void annotation()
    {
        string json = @"
            {
                ""SubVersion"": ""1707"", //子版本，1801代表着18年1月
                ""RequestOption"": ""nonvalidate"", //请求操作，validate=城市/州/邮政编码/组合验证，nonvalidate=邮政编码/州组合验证/y
                ""TransactionReference"": {
                    ""CustomerContext"": """" //客户上下文信息
                }
            }";

        JsonLoadSettings settings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Load
        };

        JToken token = JToken.Parse(json);

        // 遍历所有的注释
        foreach (var comment in token.Annotations<string>())
        {
            _testOutputHelper.WriteLine(comment); // 输出注释文本
        }

    }

    enum MappedJsonType
    {
        Object,
        Array,
        Value
    }

    private JToken dfs_json(JToken jtoken, List<ParameterNode> treeList, MappedJsonType mappedJsonType = MappedJsonType.Object)
    {
        if (treeList.Count == 0) return jtoken;
        var idxList = new List<int>();
        var hash = Json2Hash(jtoken);
        foreach (var item in hash)
        {
            _testOutputHelper.WriteLine(item.Key + "=" + item.Value);
        }
        
        switch (mappedJsonType)
        {
            case MappedJsonType.Object:
                return dfs_object(jtoken, treeList, hash, idxList);
            case MappedJsonType.Array:
                idxList.Add(-1);
                return dfs_array(jtoken, treeList[0], hash, idxList);
            case MappedJsonType.Value:
                var value = hash[treeList[0].MapAlias!];
                return value;
            default:
                throw new ArgumentOutOfRangeException(nameof(mappedJsonType), mappedJsonType, null);
        }
    }

    public static Dictionary<string, JToken> Json2Hash(JToken jtoken)
    {
        if (jtoken is JObject obj)
        {
            return Jobject2Hash(obj);
        }

        if (jtoken is JArray arr)
        {
            return JArrayToHash(arr);
        }

        // jvalue
        return new Dictionary<string, JToken>()
        {
            { jtoken.Path, jtoken }
        };
    }

    public static List<Parameter> GetMaps()
    {
        return new List<Parameter>()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "boxList",
                Type = ParameterType.Array,
                Alias = "boxList",
                //MapAlias = "order_weight"
            },
            new()
            {
                Id = 101,
                PId = 100,
                Name = "val",
                Type = ParameterType.Object,
                Alias = "boxList[*]",
            },
            new()
            {
                Id = 102,
                PId = 101,
                Name = "boxList2",
                Type = ParameterType.Array,
                Alias = "boxList[*].boxList2",
                //MapAlias = "order_weight"
            },
            new()
            {
                Id = 102,
                PId = 101,
                Name = "name",
                Type = ParameterType.String,
                Alias = "boxList[*].name",
                MapAlias = "x[*].order_weight"
            },
            new()
            {
                Id = 103,
                PId = 102,
                Name = "v",
                Type = ParameterType.Integer,
                Alias = "boxList[*].boxList2[*]",
                MapAlias = "B.b[*].c[*]",
                //IsRequired = true
            },
            new()
            {
                Id = 2,
                PId = 0,
                Name = "_B",
                Alias = "_B",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 3,
                PId = 2,
                Name = "val",
                Alias = "_B[*]",
                Type = ParameterType.Object,
            },
            new()
            {
                Id = 4,
                PId = 3,
                Name = "_D",
                Alias = "_B[*]._D",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 5,
                PId = 4,
                Name = "_E",
                Alias = "_B[*]._D[*]",
                Type = ParameterType.Integer,
                MapAlias = "B.b[*].c[*]" // 叶子
            },
            new()
            {
                Id = 6,
                PId = 3,
                Name = "_F",
                Alias = "_B[*]._F",
                Type = ParameterType.String,
                MapAlias = "B.b[*].b"
            },
            new()
            {
                Id = 7,
                PId = 0,
                Name = "_K",
                Alias = "_K",
                Type = ParameterType.Array,
            },
            new()
            {
                Id = 8,
                PId = 7,
                Name = "val",
                Alias = "_K[*]",
                Type = ParameterType.Object,
            },
            new()
            {
                Id = 9,
                PId = 8,
                Name = "_X",
                Alias = "_K[*]._X",
                Type = ParameterType.Integer,
                MapAlias = "D[*].a"
            },
            new()
            {
                Id = 10,
                PId = 8,
                Name = "_Y",
                Alias = "_K[*]._Y",
                Type = ParameterType.String,
                MapAlias = "E[*].x"
            },
        };
    }

    public static List<Parameter> GetMaps2()
    {
        // return new List<Parameter>()
        // {
        //     new()
        //     {
        //         Id = 100,
        //         PId = 0,
        //         Name = "codes",
        //         Type = ParameterType.Array,
        //         Alias = "codes",
        //     },
        //     new()
        //     {
        //         Id = 101,
        //         PId = 100,
        //         Name = "val",
        //         Type = ParameterType.Object,
        //         Alias = "codes[*]",
        //     },
        //     new()
        //     {
        //         Id = 102,
        //         PId = 0,
        //         Name = "no",
        //         Type = ParameterType.String,
        //         Alias = "no",
        //         MapAlias = "orderNo"
        //     },
        //     new()
        //     {
        //         Id = 110,
        //         PId = 101,
        //         Name = "no",
        //         Type = ParameterType.String,
        //         Alias = "codes[*].val",
        //         MapAlias = "orderNo"
        //     },
        // };
        return new List<Parameter>()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "codes",
                Type = ParameterType.Array,
                Alias = "codes",
            },
            new()
            {
                Id = 101,
                PId = 100,
                Name = "val",
                Type = ParameterType.String,
                Alias = "codes[*]",
                MapAlias = "orderNo"
            },
        };
    }

    public static List<Parameter> GetMaps3()
    {
        return new List<Parameter>()
        {
            new()
            {
                Id = 100,
                PId = 0,
                Name = "code",
                Type = ParameterType.String,
                Alias = "code",
                MapAlias = ""
            },
        };
    }

    private static Dictionary<string, JToken> Jobject2Hash(JObject jobject)
    {
        Dictionary<string, JToken> res = new();
        var q = new Queue<JProperty>(jobject.Properties());
        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            if (cur.Value is JObject obj)
            {
                foreach (var item in obj.Properties())
                {
                    q.Enqueue(item);
                }
            }
            else if (cur.Value is JArray arr)
            {
                Add(res, JArrayToHash(arr));
            }
            else
            {
                res.Add(cur.Path, cur.Value);
            }
        }

        return res;
    }

    private static List<ParameterNode> BuildTreeList(IReadOnlyCollection<Parameter> list, int pid)
    {
        var res = new List<ParameterNode>();
        foreach (var child in list.Where(x => x.PId == pid))
        {
            var node = new ParameterNode();
            node.Name = child.Name;
            node.Type = child.Type;
            node.Alias = child.Alias;
            node.IsRequired = child.IsRequired;
            if (node.Type is ParameterType.Object or ParameterType.Array)
            {
                node.Children = BuildTreeList(list, child.Id);
            }
            else
            {
                node.MapAlias = child.MapAlias;
            }

            res.Add(node);
        }

        return res;
    }

    private static Dictionary<string, JToken> JArrayToHash(JArray jarr)
    {
        Dictionary<string, JToken> res = new();
        //res.Add(jarr.Path, jarr.Count);
        for (var i = 0; i < jarr.Count; i++)
        {
            var jToken = jarr[i];
            if (jToken is JArray arr)
            {
                Add(res, JArrayToHash(arr));
            }
            else if (jToken is JObject obj)
            {
                Add(res, Jobject2Hash(obj));
            }
            else
            {
                res.Add(jToken.Path, jToken);
            }
        }

        return res;
    }

    private static void Add(Dictionary<string, JToken> a, Dictionary<string, JToken> b)
    {
        foreach (var item in b)
        {
            a.Add(item.Key, item.Value);
        }
    }


    private static JObject dfs_object(JToken jtoken, List<ParameterNode> list, Dictionary<string, JToken> hash,
        List<int> idxList)
    {
        var res = new JObject();
        foreach (var item in list)
        {
            string name = item.Name;

            if (item.Type == ParameterType.Object)
            {
                res.Add(name, dfs_object(jtoken, item.Children, hash, idxList));
            }
            else if (item.Type == ParameterType.Array)
            {
                idxList.Add(-1);
                var jarray = dfs_array(jtoken, item, hash, idxList);

                // bool isEmptyArray = jarray.All(el => el.Type == JTokenType.Null);
                // res.Add(item.Name, isEmptyArray ? null : jarray);
                res.Add(item.Name, jarray);

                idxList.RemoveAt(idxList.Count - 1);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired)
                        throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
                    res.Add(name, null);
                    continue;
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                JToken val = JValue.CreateNull();

                string path = "";

                if (item.Alias.IndexOf('[') == -1 && item.MapAlias.IndexOf('[') != -1)
                    path = item.MapAlias.Replace("[*]", "[0]");
                else
                {
                    for (int i = 0, j = 0; i < item.MapAlias.Length; i++)
                    {
                        if (item.MapAlias[i] == '[')
                        {
                            i += 2;
                            if (j >= idxList.Count)
                                throw new BusinessException(
                                    message: $"{item.Name}（{item.MapAlias} -> {item.Alias}）是非法映射");
                            path += $"[{idxList[j]}]";
                            j++;
                        }
                        else path += item.MapAlias[i];
                    }
                }

                if (hash.TryGetValue(path, out var value))
                {
                    val = value;
                }

                val = ValidationValue(item, val);
                res.Add(name, val);
            }
        }

        return res;
    }

    private static JArray dfs_array(JToken jobject, ParameterNode arr, Dictionary<string, JToken> hash,
        List<int> idxList)
    {
        var res = new JArray();

        var item = arr.Children[0];
        int len = get_len(jobject, item, idxList.Count);
        for (int idx = 0; idx < len; idx++)
        {
            idxList[^1]++;
            if (item.Type == ParameterType.Object)
            {
                res.Add(dfs_object(jobject, item.Children, hash, idxList));
            }
            else if (item.Type == ParameterType.Array)
            {
                idxList.Add(-1);
                var jarray = dfs_array(jobject, item, hash, idxList);
                res.Add(jarray);
            }
            else
            {
                if (string.IsNullOrEmpty(item.MapAlias))
                {
                    if (item.IsRequired)
                        throw new BusinessException(message: $"必填参数{item.Name}（{item.Alias}）必须建立映射");
                }

                //if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) continue;
                //var key = item.MapAlias.Replace("[*]", $"[{idx}]");

                JToken val;

                string path = "";
                var strs = item.MapAlias.Split("[*]");
                if (strs.Length < 2) path = item.MapAlias;
                var i = 0;
                for (; i < strs.Length - 1; i++)
                {
                    path += $"{strs[i]}[{idxList[i]}]";
                }

                if (i != 0) path += strs[i]; // suffix

                if (hash.ContainsKey(path!))
                {
                    val = hash[path];
                }
                else break;

                val = ValidationValue(item, val);
                res.Add(val);
            }
        }

        return res;
    }

    private static int get_len(JToken jobject, ParameterNode item, int cnt)
    {
        if (string.IsNullOrWhiteSpace(item.MapAlias))
        {
            if (item.Type != ParameterType.Array && item.Type != ParameterType.Object) return 0;
            return get_sub_len(jobject, item, cnt);
        }

        // 数组层级不对等情况
        if (item.Alias.Split("[*]").Length != item.MapAlias.Split("[*]").Length) return 1;

        int index = -1;
        for (int i = 0; i < cnt; i++)
        {
            index = item.MapAlias.IndexOf("[*]", index + 1, StringComparison.Ordinal);
            if (index == -1)
            {
                break;
            }
        }

        var path = item.MapAlias.Substring(0, index + 3);
        // for (int i = 0; i < idxList.Count; i++)
        // {
        //     var idx = path.IndexOf("[*]", StringComparison.Ordinal);
        //     if (idx >= 0)
        //     {
        //         path = path.Remove(idx,3).Insert(idx,$"[{idxList[i]}]");
        //     }
        // }
        return jobject.SelectTokens(path).Count();
    }


    private static int get_sub_len(JToken jobject, ParameterNode node, int cnt)
    {
        int res = 0;
        foreach (var item in node.Children)
        {
            var len = get_len(jobject, item, cnt);
            if (len == 0) continue;
            if (res != 0 && res != len) throw new BusinessException(message: $"组成元素数量不对等，请检查{node.Alias}的组成");
            res = Math.Max(len, res);
        }

        return res;
    }


    private static JToken ValidationValue(ParameterNode node, JToken val)
    {
        if (node.IsRequired && val.ToString() == "")
            throw new Exception(message: $"服务商参数{node.Name}是必填的（系统{node.MapAlias}->服务商{node.Alias}）");
        if (!node.IsRequired && val.Type is JTokenType.Null) return val;


        var equal = false;
        if (node.Type == ParameterType.Boolean)
        {
            equal = bool.TryParse(val.ToString(), out bool v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.Float)
        {
            equal = decimal.TryParse(val.ToString(), out decimal v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.Integer)
        {
            equal = long.TryParse(val.ToString(), out long v);
            if (equal) val = v;
        }
        else if (node.Type == ParameterType.String)
        {
            equal = val.Type is JTokenType.String;
            if (!equal)
            {
                // 数值转字符串
                if (val.Type is JTokenType.Float or JTokenType.Integer or JTokenType.Boolean)
                {
                    val = val.ToString();
                    equal = true;
                }
            }
        }


        if (!equal)
            throw new Exception(message:
                $"参数{node.Name}（{node.Alias}）期望的类型是{node.Type}，当实际分配的是{val.Type}类型的{val}（{val.Path}）");

        return val;
    }
}