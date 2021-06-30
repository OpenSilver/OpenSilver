function onResourceLoaded(resourceIndex, totalResources)
{
    document.getElementById("silverlight-loading-percentage").innerHTML = Math.round((resourceIndex / totalResources) * 100) + "%";
}

var i = 0;
var allResourcesBeingLoaded = [];
Blazor.start({ // start manually with loadBootResource
    loadBootResource: function (type, name, defaultUri, integrity) {
        if (type == "dotnetjs")
            return defaultUri;

        var fetchResources = fetch(defaultUri, {
            cache: 'no-cache',
            integrity: integrity,
            headers: { 'MyCustomHeader': 'My custom value' }
        });


        allResourcesBeingLoaded.push(fetchResources);
        fetchResources.then((r) => {
            i++;
            var total = allResourcesBeingLoaded.length;
            onResourceLoaded(i, total);
        });
        return fetchResources;
    }
});