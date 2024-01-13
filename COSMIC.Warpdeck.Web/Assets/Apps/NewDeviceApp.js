import {createApp} from 'https://unpkg.com/vue@3/dist/vue.esm-browser.js'
import WdNewDeviceForm from "/Components/Forms/NewDeviceForm.js"
let api;

let app = createApp({
    data() {
        return {
            api: null,
            availableHardware:null,
        }
    },
    methods: {
        bindHardware(){
            this.api.getAvailableHardware().then(x=>{
                this.availableHardware = x;
            });
        }
    },
    mounted() {
        api = new WarpdeckApi("http://localhost:4300/api")
        this.api = api;
        this.bindHardware();

    },
});


app.component("wdnewdeviceform", WdNewDeviceForm);
app = app.mount("#newDeviceApp");
document.app = app;

