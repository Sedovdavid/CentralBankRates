<template>
  <div v-if="this.loaded">
    <div class="container">
      <div class="input-card">
        <h3>Показать за период</h3>
        <div class="input-row">
          <label class="input-label">
            <span>С</span>
            <VueDatePicker v-model="dateFrom" locale="ru"
                           class="input-field"
                           :enable-time-picker="false"
                           :clearable="false"
                           :auto-apply="true"
                           max-width="1px"/>
          </label>
          <label class="input-label">
            <span>По</span>
            <VueDatePicker v-model="dateTo" locale="ru"
                           class="input-field"
                           :enable-time-picker="false"
                           :clearable="false"
                           :auto-apply="true"/>
          </label>
        </div>
        <button class="update-button" name="updateTable" @click="onClick">Обновить</button>
      </div>
    </div>
  </div>
  <div v-else>
    Загрузка......
  </div>
</template>

<script>
import {useCounterStore} from '@/stores/store.js'
import VueDatePicker from '@vuepic/vue-datepicker';
import '@vuepic/vue-datepicker/dist/main.css'

export default {
  data() {
    return {
      dateFrom: "",
      dateTo: "",
      loaded: false,
      counterStore: useCounterStore()
    };
  },
  components: {VueDatePicker},
  async created() {
    await this.getDefaultDates();
  },
  methods: {
    async getDefaultDates() {
      var response = await fetch('api/Currencies/GetDefaultDates');
      if (response.ok) {
        this.loaded = true;
        var dates = await response.json();
        this.dateFrom = dates.dateFrom;
        this.dateTo = dates.dateTo;
      }
    },
    onClick() {
      this.counterStore.setDates(this.dateFrom, this.dateTo);
    }
  }
};
</script>

<style scoped>
.container {
  padding-block: 1em;
  padding-inline: 5rem;
}

.input-label {
  display: flex;
  flex: auto;
  flex-direction: column;
}

.input-card {
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.3);
  width: 100%;
  padding: 2rem;
  align-items: start;
  border-radius: 1rem;
}

.input-row {
  display: flex;
  gap: 10%;
  margin-block: 1rem;
}

.input-field {
  flex: auto;
  max-width: 160px;
}

.update-button {
  border: 1px solid lightgray;
  margin-top: 1rem;
  padding: 0.5rem;
  background: #fafafa;
  border-radius: 0.5rem;
  font-size: 14px;
}

.update-button:hover {
  background-color: white;
}

.update-button:active {
  background-color: lightgray;
}

</style>
