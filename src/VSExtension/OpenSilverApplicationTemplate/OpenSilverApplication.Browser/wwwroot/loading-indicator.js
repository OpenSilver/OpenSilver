function onResourceLoaded(resourceIndex, totalResourceCount) {
    let percentage = document.getElementById("loading-indicator-percentage");
    if (percentage) {
        percentage.innerHTML = Math.round((resourceIndex / totalResourceCount) * 100) + "%";
    }
}

(function () {
    let loadingIndicatorWrapper = document.createElement("div");
    loadingIndicatorWrapper.classList.add("loading-indicator-wrapper");
    document.getElementById("app").appendChild(loadingIndicatorWrapper);

    let loadingIndicator = document.createElement("div");
    loadingIndicator.classList.add("loading-indicator");
    loadingIndicatorWrapper.appendChild(loadingIndicator);

    for (let i = 0; i < 16; i++) {
        let loadingIndicatorBall = document.createElement("div");
        loadingIndicatorBall.classList.add("loading-indicator-ball");
        loadingIndicator.appendChild(loadingIndicatorBall);
    }

    let loadingIndicatorPercentageContainer = document.createElement("div");
    loadingIndicatorPercentageContainer.classList.add("loading-indicator-percentage-container");
    loadingIndicator.appendChild(loadingIndicatorPercentageContainer);

    let loadingIndicatorPercentage = document.createElement("div");
    loadingIndicatorPercentage.id = "loading-indicator-percentage";
    loadingIndicatorPercentageContainer.appendChild(loadingIndicatorPercentage);
})();