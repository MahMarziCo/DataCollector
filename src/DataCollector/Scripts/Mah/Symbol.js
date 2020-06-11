SymbolClass = {
    renderSymbol : function (data) {
        var templateContent = $("#symbol-template").html();
        var template = kendo.template(templateContent);

        return template(data);
    }
}