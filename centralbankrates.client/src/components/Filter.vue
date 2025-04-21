<template>
  <div v-if="this.loaded">
    <div class="container">
      <div class="input-card">
        <h3>Показать за период</h3>
        <div class="input-row">
          <label class="input-label">
            <span>С</span>
            <input v-model="dateFrom" type="date" class="input-field"/>
          </label>
          <label class="input-label">
            <span>По</span>
            <input v-model="dateTo" type="date" class="input-field"/>
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
import { useCounterStore } from '@/stores/store.js'
export default {
  data() {
    return {
      dateFrom: "",
      dateTo: "",
      loaded: false,
      counterStore: useCounterStore()
    };
  },
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
  padding-inline: 4rem;
}

.input-label {
  display: flex;
  flex: auto;
  flex-direction: column;
}

.input-card {
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
  width: 100%;
  padding: 1.5rem;
  align-items: start;
}

.input-row {
  display: flex;
  gap: 10%;
  margin-block: 1rem;
}

.input-field {
  flex: auto;
  padding: 0.5rem;
  border: 1px solid lightgray;
  font-size: 14px;
  font-family: Arial, serif;
}

.update-button {
  border: 1px solid lightgray;
  margin-top: 1rem;
  padding: 10px;
  background: #fafafa;
}

.update-button:hover {
  background-color: white;
}

.update-button:active {
  background-color: lightgray;
}

</style>
