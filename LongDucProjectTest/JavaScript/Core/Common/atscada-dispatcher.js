
export class AtscadaEvent {
    constructor(eventName) {
        this.eventName = eventName;
        this.callbacks = [];
    }

    register(callback) {
        this.callbacks.push(callback);
    }

    unregister(callback) {
        const index = this.callbacks.indexOf(callback);
        if (index > -1) {
            this.callbacks.splice(index, 1);
        }
    }

    async fire(data) {
        const callbacks = this.callbacks.slice(0);
        for (const callback of callbacks) {
            if (callback.constructor.name === 'AsyncFunction')
                await callback(data);
            else
                callback(data);
        }
    }
}

export class AtscadaDispatcher {
    constructor() {
        this.events = {};
    }

    dispatch(eventName, data) {
        const event = this.events[eventName];
        if (event) {
            event.fire(data);
        }
    }

    on(eventName, callback) {
        let event = this.events[eventName];
        if (!event) {
            event = new AtscadaEvent(eventName);
            this.events[eventName] = event;
        }
        event.register(callback);
    }

    off(eventName, callback) {
        const event = this.events[eventName];
        if (event && event.callbacks.indexOf(callback) > -1) {
            event.unregister(callback);
            if (event.callbacks.length === 0) {
                delete this.events[eventName];
            }
        }
    }
}