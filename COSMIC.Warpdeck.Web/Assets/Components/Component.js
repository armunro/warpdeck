export default {
    props: {
        deviceId: String,
        layerId: String,
        keyId: String
    },
    data() {
        return {}
    },
    methods: {
        calculateIconUrl() {
            let ms = Date.now();
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/icon/" + this.keyId;
        }
    },
    mounted() {
    },
    template: `
      <div id="key_25" class="col m-0 p-0 keyMapKey" data-keyid="25" data-deviceid="Office Streamdeck XL"
           data-layerid="active" data-keybound="true" draggable="true">
        <img class="img-fluid me-2 mb-2 keyMapKeyImage"
             style="border-radius: 1rem"
             :src="calculateIconUrl()">
      </div>
    `
}
