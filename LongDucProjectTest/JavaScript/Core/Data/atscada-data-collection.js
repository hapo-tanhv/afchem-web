
import { AtscadaDataTag } from './atscada-data-tag.js'

export class AtscadaDataCollection {
    constructor(task) {
        this.task = task;
        this.map = new Map();
    }
    
    getAll() {
        return [...this.map.values()]
    }

    get(name) {
        return this.map.get(name);
    }

    add(name) {
        if (this.map.has(name)) return false;
        this.map.set(name, new AtscadaDataTag(name, this.task));
        return true;
    }

    remove(name) {
        return this.map.delete(name);
    }

    contains(name) {
        return this.map.has(name);
    }

    clear() {
        this.map.clear();
    }
}