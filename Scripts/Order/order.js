$(document).ready(function () {

    $("#addOrder").attr("disabled", true);
    $("#moreQtd").attr("disabled", true);
    $("#lessQtd").attr("disabled", true);
    $("#clearPreListOrder").attr("disabled", true);
    $("#addItemList").attr("disabled", true);
    $("#totalPriceListOrder").html("TOTAL R$ 0");

    $("#selectedItem").change(function () {
        if ($("#selectedItem")[0].selectedIndex === 0) {
            $("#addItemList").attr("disabled", true);
            $("#totalPrice").html("");
            $("#quantity").html(1);
        }
        else {
            $.ajax({
                url: "/Order/CheckPrice",
                method: "POST",
                data: {
                    idItem: $("#selectedItem")[0].selectedIndex
                },
                success: function (price) {
                    $("#moreQtd").attr("disabled", false);
                    $("#lessQtd").attr("disabled", false);
                    $("#addItemList").attr("disabled", false);
                    $("#clearPreListOrder").attr("disabled", false);

                    $("#price").val(price);
                    $("#quantity").html(1);
                    totalPrice = $("#quantity").html() * price;
                    $("#totalPrice").html(totalPrice.toLocaleString("pt-br", { style: "currency", currency: "BRL" }))
                }, error: function () {

                }
            });
        }
    })


    $("#addOrder").click(function () {

        item = $("#selectedItem")[0].selectedOptions[0].textContent;

        $.ajax({
            url: "/Order/AddOrder",
            method: "POST",
            async: false,
            data: {
                listItensJSON: JSON.stringify(listItems)
            },
            success: function (r) {
                alert("Pedido: " + r);
            }, error: function () {

            }
        });

        listItems = new Array();
        cont = 1;
        $("#selectedItem")[0].selectedIndex = 0;
        $("#preListOrder").html("");
        $("#moreQtd").attr("disabled", true);
        $("#addOrder").attr("disabled", true);
        $("#lessQtd").attr("disabled", true);
        $("#addItemList").attr("disabled", true);
        $("#price").val(price);
        $("#quantity").html(1);
        $("#totalPrice").html("");
        $("#clearPreListOrder").attr("disabled", false);
        $("#totalPriceListOrder").html("TOTAL R$ 0");
    })

    $("#moreQtd").click(function () {
        quantity = parseInt($("#quantity").html()) + 1;
        $("#quantity").html(quantity);

        totalPrice = $("#price").val() * quantity;
        $("#totalPrice").html(totalPrice.toLocaleString("pt-br", { style: "currency", currency: "BRL" }))

    });

    $("#lessQtd").click(function () {
        if (parseInt($("#quantity").html()) !== 1) {
            quantity = parseInt($("#quantity").html()) - 1;
            $("#quantity").html(quantity);

            totalPrice = $("#price").val() * quantity;
            $("#totalPrice").html(totalPrice.toLocaleString("pt-br", { style: "currency", currency: "BRL" }))
        }
    });


    cont = 1;
    listItems = new Array();
    $("#addItemList").click(function () {
        item = new Object();
        item.cont = cont++;
        item.name = $("#selectedItem")[0].selectedOptions[0].textContent;
        item.timeDelivery = parseInt($("#selectedItem").val());
        item.quantity = parseInt($("#quantity").html());
        item.price = parseFloat($("#price").val()).toFixed(2);
        item.totalPrice = parseFloat($("#price").val() * $("#quantity").html()).toFixed(2);
        listItems.push(item);

        $("#quantity").html(1);
        $("#totalPrice").html("")
        $("#preListOrder").html("");
        $("#addItemList").attr("disabled", true);
        $("#addOrder").attr("disabled", false);
        $("#moreQtd").attr("disabled", true);
        $("#lessQtd").attr("disabled", true);
        $("#selectedItem")[0].selectedIndex = 0;
        var totalPriceList = 0;
        for (i = 0; i < listItems.length; i++) {

            totalPriceList = totalPriceList + listItems[i].price * listItems[i].quantity
            $("#preListOrder").append("<tr><td>" + listItems[i].cont + "</td><td>" + listItems[i].name + "</td><td>" + listItems[i].quantity + "</td><td>" + (listItems[i].price * listItems[i].quantity).toLocaleString("pt-br", { style: "currency", currency: "BRL" }) + "</td></tr>")

        }
        $("#totalPriceListOrder").html("TOTAL " + totalPriceList.toLocaleString("pt-br", { style: "currency", currency: "BRL" }));

    })

    $("#clearPreListOrder").click(function () {
        cont = 1;
        listItems = new Array();
        $("#selectedItem")[0].selectedIndex = 0;
        $("#preListOrder").html("");
        $("#moreQtd").attr("disabled", true);
        $("#addOrder").attr("disabled", true);
        $("#lessQtd").attr("disabled", true);
        $("#addItemList").attr("disabled", true);
        $("#price").val(price);
        $("#quantity").html(1);
        $("#totalPrice").html("");
        $("#clearPreListOrder").attr("disabled", true);
        $("#totalPriceListOrder").html("TOTAL R$ 0");

    })

});

