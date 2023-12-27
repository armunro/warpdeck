export default {
    props: {
        deviceId: String,
        layerId: String,
        componentId: String
    },
    data() {
        return {}
    },
    methods: {
    },
    mounted() {
    },
    template: `
      <div id="propertiesCard" class="active">
        <div class="card mb-2">
          <div class="card-header">
            Behavior
          </div>
          <div class="card-body">
            <select id="behaviorSelect" class="form-select d-none">
              <option value="Press">Press</option>
              <option value="PressAndHold">Press And Hold</option>
            </select>
            <div id="actionContainer" class="form-group">
              <div>
                <div id="action_Press_params" class="form-group">
                  <div class="input-group mb-2"><span class="input-group-text">Press Action</span><select
                      id="action_Press_type" class="form-select">
                    <option value="PasteAction">PasteAction</option>
                    <option value="WebLaunchAction">WebLaunchAction</option>
                    <option value="ManageWindowAction">ManageWindowAction</option>
                    <option value="LauncherAction">LauncherAction</option>
                    <option selected="" value="KeyMacro">KeyMacro</option>
                  </select></div>
                </div>
                <div>
                  <div class="form-group">
                    <div class="input-group ps-4 mb-2"><span class="input-group-text">keys</span><input type="text"
                                                                                                        id="action_Press_param_keys"
                                                                                                        class="form-control"
                                                                                                        value="(mediaplaypause)">
                    </div>
                  </div>
                </div>
              </div>
              <div>
                <div id="action_Hold_params" class="form-group">
                  <div class="input-group mb-2"><span class="input-group-text">Hold Action</span><select
                      id="action_Hold_type" class="form-select">
                    <option value="PasteAction">PasteAction</option>
                    <option value="WebLaunchAction">WebLaunchAction</option>
                    <option value="ManageWindowAction">ManageWindowAction</option>
                    <option value="LauncherAction">LauncherAction</option>
                    <option value="KeyMacro">KeyMacro</option>
                  </select></div>
                </div>
              </div>
            </div>

            <form class="form">
              <div id="tagContainer" class="form-group">
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <div class="form-group mb-2">
                          <label for="keyId">Category</label><input type="text" id="tag_Category" data-tag="Category"
                                                                    class="form-control keyTag" value="Multimedia">
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div>
                  <div class="card mb-2">
                    <div class="card-header">
                      Properties
                    </div>
                    <div class="card-body">
                      <div>
                        <div class="form-group mb-2">
                          <label for="keyId">Text</label><input type="text" id="tag_Text" data-tag="Text"
                                                                class="form-control keyTag" value="PLAY"></div>
                      </div>
                      <div>
                        <div class="form-group mb-2">
                          <label for="keyId">Icon</label><input type="text" id="tag_Icon" data-tag="Icon"
                                                                class="form-control keyTag"
                                                                value="fontawesome-pro-6.4.0-web\\svgs\\solid\\play-pause.svg">
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <div class="input-group">
                <select class="form-select" id="addPropertyNameSelect">
                  <option value="Category">Category</option>
                  <option value="Text">Text</option>
                  <option value="BackgroundColor">Background Color</option>
                  <option value="IconColor">Graphic Color</option>
                  <option value="Icon">Icon</option>
                  <option value="AccentColor">Accent Color</option>
                </select>
                <input type="text" id="addPropertyValueTextBox" class="form-control">
                <button class="btn btn-outline-secondary" id="addPropertyButton" type="button">
                  Add
                  Property
                </button>
              </div>
              <div class="form-group">
                <button id="saveKey" type="button" class="btn btn-primary">Save</button>
                <button id="deleteKey" type="button" class="btn btn-primary">Delete</button>
              </div>
            </form>
          </div>
        </div>

      </div>
    `
}
