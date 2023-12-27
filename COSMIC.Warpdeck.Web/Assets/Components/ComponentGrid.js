export default {
    props: {
        deviceid: String,
        layerid: String,
        columns: Number,
        rows: Number

    },
    data() {
        return {
            activeComponent: null
        }
    },
    methods: {
        range(end) {
            return Array.from({length: end}, (_, i) => i + 1)
        },
        calculateLayerKeysUri() {
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key"
        },

        calculateLayerKeyUri(keyId) {
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key/" + keyId
        },
        component_clicked(clickedComponent) {
            if (this.activeComponent)
                this.activeComponent.deactivate();
            clickedComponent.activate();
            this.activeComponent = clickedComponent;
        }
    },
    mounted() {
    },
    template: `
      <div class="row" v-for='r in range(this.rows)'>
        <wdcomponent :deviceId="this.deviceid" :layerId="this.layerid"
                     :keyId="(((r-1) * this.columns) + (c-1)).toString()"
                     @component-click="component_clicked"
                     v-for="c in range(this.columns)"></wdcomponent>
      </div>
    `
}
