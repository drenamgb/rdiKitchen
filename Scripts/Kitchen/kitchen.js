$(document).ready(function () {


    setInterval(function () {

        $.ajax({
            url: "/Kitchen/UpdateListOrder",
            method: "POST",
            async: false,
            success: function (rs) {
                waitList(rs.waitListOrder);
                deliveryList(rs.deliveryListOrder);
            }, error: function (rs) {
                alert(rs.data);
            }
        })

    }, 1000);

    $("#btnClearListDelivery").click(function () {
        $.ajax({
            url: "/Kitchen/DeleteDeliveryList",
            method: "POST",
            success: function (rs) {
            }, error: function (rs) {
                alert(rs);
            }
        })
    })

    function ConvertDateTime(startdate) {
        var dateTimeMilliseconds = startdate.slice(6, startdate.length - 2);
        var dateReal = new Date(parseInt(dateTimeMilliseconds));

        var timeStart = new Date().getTime();
        var timeEnd = new Date(dateReal).getTime();
        var hourDiff = timeEnd - timeStart; //in ms
        var secDiff = hourDiff / 1000; //in s

        return parseInt(secDiff);
    }

    function waitList(waitList) {
        $("#waitList").html("");
        $("#waitList").append("<thead>");
        $("#waitList").append("<tr><th>Pedido</th><th>Qtd / Item Tempo de Preparo</th></tr>");
        $("#waitList").append("<tbody>");

        if (waitList.length === 0) {
            $("#waitList").append("<tr><td align='center' colspan='2'><font size='2px' color='#222222'><strong>... Aguardando</strong></font></td></tr>");
        }

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

                detalhesItens += "<strong>" + waitList[i].ListItens[j].Quantity + "</strong> " + waitList[i].ListItens[j].Name + " " + relogio;

                detalhesItens += "<br>";
            }

            $("#waitList").append("<tr><td align='center'><font size='4px'><strong>" + waitList[i]["IdOrder"] + "</strong></font></td><td align='left'>" + detalhesItens + "</td></tr>");
        }
    }

    function deliveryList(deliveryList) {
        $("#deliveryList").html("");
        $("#deliveryList").append("<thead>");
        $("#deliveryList").append("<tr><th>Pedido</th><th>Qtd / Item</th></tr>");
        $("#deliveryList").append("<tbody>");

        if (deliveryList.length === 0) {

            $("#deliveryList").append("<tr><td align='center' colspan='2'><font size='2px' color='#222222'><strong>... Em preparo</strong></font></td></tr>");
        }

        var img = "<img src='/content/img/ok.png' width='15px'>";
        for (i = 0; i < deliveryList.length; i++) {
            var detalhesItens = "";
            for (j = 0; j < deliveryList[i].ListItens.length > 0; j++) {

                detalhesItens += "<strong>" + deliveryList[i].ListItens[j].Quantity + "</strong>  " + deliveryList[i].ListItens[j].Name + " " + img;
                detalhesItens += "<br>";
            }

            $("#deliveryList").append("<tr><td align='center'><font size='4px'><strong>" + deliveryList[i]["IdOrder"] + "</strong></font></td><td align='left'>" + detalhesItens + "</td></tr>");
        }
    }

});
