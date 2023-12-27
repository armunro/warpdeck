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
      <div id="key_{{this.keyId}}" class="col m-0 p-0 keyMapKey" draggable="true">
        <img class="img-fluid me-2 mb-2 keyMapKeyImage"
             :src="calculateIconUrl()">
      </div>
    `
}
