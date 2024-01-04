export default {
    props: {}, data() {
        return {
            availableHardware:[],
            entered: {
                name: "",
                hardwareId: "",
            }
        }
    }, methods: {
        on_create_click(){
            
        }
    }, mounted() {
    }, template: `
    <form class="form">
      <div class="mb-3">
        <input type="radio" class="btn-check" name="deviceActionNameOptions"
               checked>
        <label class="btn btn-secondary" for="virtualDeviceRadio">Virtual</label>
        <input type="radio" class="btn-check" name="deviceActionNameOptions">
        <label class="btn btn-secondary" for="hardwareDeviceRadio">Hardware</label>
      </div>
      <div class="input-group mb-3">
        <span class="input-group-text" >Hardware Device</span>
        <select class="form-select" v-model="entered.hardwareId">
          <option v-for="hardware in availableHardware" :value="hardware.hardwareId">{{ hardware.name }}</option>
        </select>
      </div>
      <div class="input-group mb-3">
        <span class="input-group-text" >Device Name</span>
        <input  type="text" class="form-control" v-model="entered.name">
      </div>
      <div class="input-group">
        <button class="btn btn-primary" @click="on_create_click" type="button">Create</button>
      </div>
    </form>
    `
}
