export default {
    props: {
        componentModel: Object,
        columns: Number,
        rows: Number
    },
    data (){
        return {
        }
    },
    methods: {
        calculateIconUrl() {
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/icon";
        },
        calculateLayerKeysUri() {
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key"
        },

        calculateLayerKeyUri(keyId) {
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/key/" + keyId
        }
    },
    mounted() {
    },
    template: `
        <div v-for="r in rows">
            <div v-for="c in columns">
            hi
            </div>
        </div>
    `
}
