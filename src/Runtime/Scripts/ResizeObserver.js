 /**
  * The throttle pattern is more suitable for events that are triggered many times in a short period of time. 
  * This technique is normally used to control scrolling, resizing and mouse-related events. 
  * By using throttle, we can filter repeated executions of an event handler, enforcing a minimum wait time between calls.
  * 
  * @param {Function} callback 
  * @param {Number} interval 
*/
function throttle(callback, interval) {
    let enableCall = true;

    return function(...args) {
      if (!enableCall) return;

      enableCall = false;
      callback.apply(this, args);
      setTimeout(() => enableCall = true, interval);
    }
}

/**
 * The debounce pattern delays the calling of the event handler until a pause happens. 
 * This technique is commonly used in search boxes with a suggest drop-down list. 
 * By applying this pattern, we can prevent unnecessary requests to the backend while the user is typing.
 * 
 * @param {Function} callback 
 * @param {Number} interval 
 */
 function debounce(callback, interval) {
    let debounceTimeoutId;

    return function(...args) {
      clearTimeout(debounceTimeoutId);
      debounceTimeoutId = setTimeout(() => callback.apply(this, args), interval);
    };
}

class ResizeObserverAdapter {

    constructor() {
        this.observer = new ResizeObserver(entries => this.onResize(entries));
        this.callbacks = {};
    }

    observe(element, callback) {
        if (element && element.id) {
            this.observer.observe(element);
            this.callbacks[element.id] = debounce(callback, 100);
        }
    }

    unobserve(element) {
        if (element && element.id) {
            this.observer.unobserve(element);
            delete this.callbacks[element.id];
        }
    }

    onResize(resizedElements) {
        for (const element of resizedElements) {
            if (this.callbacks[element.target.id]) {
                this.callbacks[element.target.id](element.contentRect.width + '|' + element.contentRect.height);
            }
        }
    }
}