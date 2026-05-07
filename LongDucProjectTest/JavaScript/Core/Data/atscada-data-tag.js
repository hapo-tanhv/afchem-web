
import { AtscadaDispatcher } from '../Common/atscada-dispatcher.js'
import { AtscadaDataCommand } from './atscada-data-command.js'

export class AtscadaDataTag {
    constructor(name, task) {
        this.name = name;
        this.value = null;
        this.status = '';
        this.timeStamp = '';
        this.dispatcher = new AtscadaDispatcher();

        this.dataReader = task.dataReader;
        this.dataWriter = task.dataWriter;
    }

    read() {
        return this.dataReader.read(this.name);
    }

    write(value, callback) {
        const command = new AtscadaDataCommand({
            name: this.name,
            value: String(value),
            result: (resultPackage) =>
                this.onEventWriteCompleted(callback, resultPackage)
        });
        this.dataWriter.write(command);
    }

    update(resultPackage) {
        resultPackage = resultPackage || {};
        if (resultPackage['Name'] !== this.name) return;

        this.timeStamp = resultPackage['TimeStamp'];
        const oldValue = this.value;
        const oldStatus = this.status;

        if (this.value !== resultPackage['Value']) {
            this.value = resultPackage['Value'];
            this.onEventValueStatusChange(oldValue, resultPackage['Value']);
        }
        if (this.status !== resultPackage['Status']) {
            this.status = resultPackage['Status'];
            this.onEventStatusChanged(oldStatus, resultPackage['Status']);
        }
    }

    onEventValueStatusChange(oldValue, newValue) {
        if (oldValue === newValue) return;
        this.dispatcher.dispatch('valueChanged', {
            sender: this,
            e: {
                oldValue: oldValue,
                newValue: newValue
            }
        });
    }

    onEventStatusChanged(oldStatus, newStatus) {
        if (oldStatus === newStatus) return;
        this.dispatcher.dispatch('statusChanged', {
            sender: this,
            e: {
                oldStatus: oldStatus,
                newStatus: newStatus
            }
        });
    }

    onEventWriteCompleted(callback, resultPackage) {
        resultPackage = resultPackage || {};
        if (this.name !== resultPackage['Name']) return;
        this.dispatcher.dispatch('writeCompleted', {
            sender: this,
            callback: callback,
            e: {
                value: resultPackage['Value'],
                status: resultPackage['Status'],
                timeStamp: resultPackage['TimeStamp']
            }
        });
    }
}