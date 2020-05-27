$(document).ready(function () {

    $("#deliveryList").html("");

    setInterval(function () {

        $.ajax({
            url: "/Kitchen/UpdateOrder",
            method: "POST",
            async: false,
            success: function (rs) {
                waitList(rs.waitListOrder);
                deliveryList(rs.deliveryListOrder);
            }, error: function (rs) {
                alert(rs);
            }
        })

    }, 1000);



    function ConvertDateTime(startdate) {
        var dateTimeMilliseconds = startdate.slice(6, startdate.length - 2);
        var dateReal = new Date(parseInt(dateTimeMilliseconds));

        var timeStart = new Date().getTime();
        var timeEnd = new Date(dateReal).getTime();
        var hourDiff = timeEnd - timeStart; //in ms
        var secDiff = hourDiff / 1000; //in s
        //var minDiff = hourDiff / 60 / 1000; //in minutes
        //var hDiff = hourDiff / 3600 / 1000; //in hours
        //var humanReadable = {};
        //humanReadable.hours = Math.floor(hDiff);
        //humanReadable.minutes = minDiff - 60 * humanReadable.hours;

        return parseInt(secDiff);
    }

    function waitList(waitList) {
        $("#waitList").html("");
        $("#waitList").append("<thead>");
        $("#waitList").append("<tr><th>Pedido</th><th>Qtd / Item</th><th>Tempo de Preparo</th></tr>");
        $("#waitList").append("<tbody>");

        for (i = 0; i < waitList.length; i++) {
            var relogio;
            var detalhesItens = "";
            for (j = 0; j < waitList[i].ListItens.length > 0; j++) {

                var timeRest = ConvertDateTime(waitList[i].ListItens[j].HourEnd);

                if (timeRest > 0) {
                    relogio = " <img src='/content/img/wait.png' width='15px'> " + timeRest + "s";
                } else {
                    relogio = " <img src='/content/img/ok.png' width='15px'>"
                }

                detalhesItens +=  waitList[i].ListItens[j].Quantity + "  " + waitList[i].ListItens[j].Name;

                detalhesItens += "<br>";
            }

            $("#waitList").append("<tr><td>" + waitList[i]["IdOrder"] + "</td><td>" + detalhesItens + "</td><td>"+relogio+"</td></tr>");
        }
    }

    function deliveryList(deliveryList) {
        $("#deliveryList").html("");
        $("#deliveryList").append("<thead>");
        $("#deliveryList").append("<tr><th>Pedido</th><th>Qtd / Item</th></tr>");
        $("#deliveryList").append("<tbody>");

        var img = "<img src='/content/img/ok.png' width='15px'>";
        for (i = 0; i < deliveryList.length; i++) {
            var detalhesItens = "";
            for (j = 0; j < deliveryList[i].ListItens.length > 0; j++) {

                detalhesItens +=  deliveryList[i].ListItens[j].Quantity + "  " + deliveryList[i].ListItens[j].Name + img;
                detalhesItens += "<br>";
            }

            $("#deliveryList").append("<tr><td>" + deliveryList[i]["IdOrder"] + "</td><td>" + detalhesItens + "</td></tr>");
        }
    }

});
