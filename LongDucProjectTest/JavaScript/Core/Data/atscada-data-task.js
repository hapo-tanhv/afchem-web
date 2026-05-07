
import { AtscadaDispatcher } from '../Common/atscada-dispatcher.js'
import { AtscadaDataService } from './atscada-data-service.js'
import { AtscadaDataReader } from './atscada-data-reader.js'
import { AtscadaDataWriter } from './atscada-data-writer.js'
import { AtscadaDataCollection } from './atscada-data-collection.js'

export class AtscadaDataTask {
    constructor(taskParam) {
        taskParam = taskParam || {};
        var i = 0;
        this.pollingID = undefined;
        this.operatingStatus = false;
        this.interval = taskParam.interval || 1000;
        this.maxNumberOfWrite = taskParam.maxNumberOfWrite || 10;
        this.timeout = taskParam.timeout || 900;

        this.dispatcher = new AtscadaDispatcher();
        this.dataCollection = new AtscadaDataCollection(this);
        this.dataService = new AtscadaDataService(this.timeout);
        this.dataReader = new AtscadaDataReader(this.dataService);
        this.dataWriter = new AtscadaDataWriter(this.dataService, this.maxNumberOfWrite);
    }

    start() {
        if (this.operatingStatus) return;
        this.operatingStatus = true;
        this.polling();
    }

    stop() {
        this.operatingStatus = false;
        if (this.pollingID)
            clearTimeout(this.pollingID);
    }

    polling() {
        this.pollingID = setTimeout(async () => {
            if (!this.operatingStatus) return;
            this.onEventRefresh('beforeRefresh');
            try {
                const dataTags = this.dataCollection.getAll();
                await this.dataWriter.update();
                await this.dataReader.update(dataTags.map(x => x.name));
                for (const dataTag of dataTags) {
                    var data = dataTag.read();
                    dataTag.update(data);
                }
            }
            catch (e) {
                console.log(e.message);
            }
            finally {
                if (!this.operatingStatus) return;
                this.onEventRefresh('refreshed');
                this.polling();
            }
        }, this.interval);
    }

    onEventRefresh(eventName) {
        this.dispatcher.dispatch(eventName, {
            sender: this,
            e: {
                timeStamp: new Date()
            }
        });
    }
}
