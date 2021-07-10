class ResizeObserverAdapter {

    constructor() {

        this.observer = new ResizeObserver(entries => this.onResize(entries));
        this.callbacks = {};
    }

    observe(element, callback) {

        this.observer.observe(element);
        this.callbacks[element] = callback;
    }

    unobserve(element) {

        this.observer.unobserve(element);
        delete this.callbacks(element);
    }

    onResize(resizedElements) {

        for (const element of resizedElements) {

            this.callbacks[element.target](element.contentRect.width, element.contentRect.height);
        }
    }
}