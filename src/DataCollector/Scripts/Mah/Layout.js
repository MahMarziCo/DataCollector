Layout_Class ={
    ShowConfirm: function (title,message,actions) {
        $("#layoutDialog").kendoDialog({
            width: "300px",
            title: title,
            closable: false,
            visible:false,
            content: "<p>"+message+"<p>",
            actions: actions});
        $("#layoutDialog").data("kendoDialog").open();
    },
    ShowAlert: function (message,title) {
        if (!title)
        {
            title = "خطا";
        }
        $("#layoutDialog").kendoDialog({
            width: "300px",
            title: title,
            closable: false,
            visible: false,
            content: "<p>" + message + "<p>",
            actions: [{text:"تایید"}]
        });
        $("#layoutDialog").data("kendoDialog").open();
    }
}

$().ready(function () {

    if ($("#layoutDialog").length == 0) {
        $("body").append("<div id='layoutDialog'></div>")
    }
})