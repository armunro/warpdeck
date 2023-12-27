export default {
    props: {
        deviceId: String,
        layerId: String,
        keyId: String
    },
    data() {
        return { isActive: false}
    },
    methods: {
        calculateIconUrl() {
            let ms = Date.now();
            return "/api/device/" + this.deviceId + "/layer/" + this.layerId + "/icon/" + this.keyId;
        },
        activate(){
            this.isActive = true;
        },
        deactivate(){
            this.isActive = false ;
        },
        onComponentIconClick(item) {
            this.$emit('component-click', item);
        }
        
    },
    mounted() {
    },
    template: `
      <div id="key_{{this.keyId}}" :class="{ active: isActive}" class="col m-0 p-0 keyMapKey" draggable="true">
        <img alt="" class="img-fluid me-2 mb-2 keyMapKeyImage" @click="onComponentIconClick(this)" :src="calculateIconUrl()"/>
      </div>
    `
}
