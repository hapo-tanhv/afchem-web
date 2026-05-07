
export class AtscadaDataCommand {
    constructor(param) {
        this.name = param.name;
        this.value = param.value;      
        this.result = param.result;

        this.isCompleted = false;
        this.numberOfWrite = 0;
        this.maxNumberOfWrite = 10;
    }

    update(resultPackage) {       
        resultPackage = resultPackage || {};
        if (resultPackage['Name'] === this.name &&
            resultPackage['Value'] === this.value &&
            resultPackage['Status'] === 'Good') {
            this.result(resultPackage);
            this.isCompleted = true;
        }
        else {
            this.numberOfWrite++;
            if (this.numberOfWrite < this.maxNumberOfWrite)
                return;
            resultPackage['Name'] = this.name;
            resultPackage['Value'] = this.value;
            this.result(resultPackage);
            this.isCompleted = true;
        }        
    }
}