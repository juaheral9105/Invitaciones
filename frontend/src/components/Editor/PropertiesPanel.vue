<template>
  <div v-if="selectedBlock" class="p-4">
    <div class="flex items-center justify-between mb-4 pb-3 border-b">
      <h2 class="text-lg font-semibold">{{ getBlockLabel(selectedBlock.type) }}</h2>
      <button
        @click="store.deselectBlock()"
        class="text-gray-400 hover:text-gray-600"
      >
        ✕
      </button>
    </div>

    <div class="space-y-4">
      <!-- Common Properties: Styles (ocultar para form) -->
      <div v-if="selectedBlock.type !== 'form'" class="space-y-3">
        <h3 class="text-sm font-semibold text-gray-700">Estilos</h3>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Color de texto</label>
          <input
            v-model="selectedBlock.style.color"
            type="color"
            class="w-full h-9 rounded cursor-pointer"
            @change="updateBlockStyle"
          />
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Color de fondo</label>
          <input
            v-model="selectedBlock.style.backgroundColor"
            type="color"
            class="w-full h-9 rounded cursor-pointer"
            @change="updateBlockStyle"
          />
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Fuente</label>
          <select
            v-model="selectedBlock.style.fontFamily"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            @change="updateBlockStyle"
            :style="{ fontFamily: selectedBlock.style.fontFamily }"
          >
            <optgroup label="Sans-serif (modernas)">
              <option value="Arial">Arial</option>
              <option value="Montserrat">Montserrat</option>
              <option value="Open Sans">Open Sans</option>
              <option value="Raleway">Raleway</option>
              <option value="Poppins">Poppins</option>
              <option value="Roboto">Roboto</option>
              <option value="Oswald">Oswald</option>
              <option value="Bebas Neue">Bebas Neue</option>
            </optgroup>
            <optgroup label="Serif (elegantes)">
              <option value="Playfair Display">Playfair Display</option>
              <option value="Lora">Lora</option>
              <option value="Merriweather">Merriweather</option>
              <option value="Crimson Text">Crimson Text</option>
            </optgroup>
            <optgroup label="Script (caligráficas)">
              <option value="Dancing Script">Dancing Script</option>
              <option value="Pacifico">Pacifico</option>
              <option value="Great Vibes">Great Vibes</option>
              <option value="Satisfy">Satisfy</option>
            </optgroup>
            <optgroup label="Monospace">
              <option value="Courier New">Courier New</option>
            </optgroup>
          </select>
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Tamaño de fuente</label>
          <input
            v-model="selectedBlock.style.fontSize"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="16px"
            @change="updateBlockStyle"
          />
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Alineación</label>
          <div class="flex gap-2">
            <button
              v-for="align in ['left', 'center', 'right']"
              :key="align"
              @click="selectedBlock.style.textAlign = align; updateBlockStyle()"
              :class="[
                'flex-1 px-3 py-2 text-sm rounded border',
                selectedBlock.style.textAlign === align
                  ? 'bg-purple-600 text-white border-purple-600'
                  : 'bg-white text-gray-700 border-gray-300 hover:bg-gray-50'
              ]"
            >
              {{ align === 'left' ? '⬅️' : align === 'center' ? '↔️' : '➡️' }}
            </button>
          </div>
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Ancho del contenedor</label>
          <input
            v-model="selectedBlock.style.width"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="100%, 600px, auto"
            @change="updateBlockStyle"
          />
        </div>

        <div>
          <label class="block text-xs font-medium text-gray-600 mb-1">Alto del contenedor</label>
          <input
            v-model="selectedBlock.style.height"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="auto, 300px"
            @change="updateBlockStyle"
          />
        </div>

        <!-- Decoration Section for all blocks except form, divider and spacer -->
        <div v-if="selectedBlock.type !== 'divider' && selectedBlock.type !== 'spacer'" class="pt-4 border-t">
          <h3 class="text-sm font-semibold text-gray-700 mb-3">🎨 Decoración de Fondo</h3>

          <!-- Enable decoration toggle -->
          <div class="flex items-center justify-between mb-3">
            <label class="text-xs text-gray-700 font-medium">Activar decoración</label>
            <input
              type="checkbox"
              v-model="selectedBlock.content.decorationEnabled"
              @change="updateBlockContent"
              class="w-4 h-4 text-purple-600 rounded focus:ring-purple-500"
            />
          </div>

          <div v-if="selectedBlock.content.decorationEnabled" class="space-y-3 pt-2 border-t">
            <!-- Upload decoration image -->
            <div>
              <label class="block text-xs font-medium text-gray-600 mb-2">Imagen de decoración</label>
              <input
                type="file"
                @change="uploadBlockDecoration"
                accept="image/*"
                class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
              />
              <p v-if="uploading" class="text-xs text-blue-600 mt-1">Subiendo imagen...</p>

              <!-- Preview of current decoration -->
              <div v-if="selectedBlock.content.decorationImage" class="mt-2">
                <img
                  :src="selectedBlock.content.decorationImage"
                  alt="Decoración"
                  class="w-full h-24 object-contain rounded border border-gray-200"
                />
                <button
                  @click="selectedBlock.content.decorationImage = ''; updateBlockContent()"
                  class="mt-1 text-xs text-red-600 hover:text-red-800"
                >
                  Eliminar imagen
                </button>
              </div>
            </div>

            <!-- Position selection -->
            <div>
              <label class="block text-xs font-medium text-gray-600 mb-2">Posición</label>
              <select
                v-model="selectedBlock.content.decorationPosition"
                @change="updateBlockContent"
                class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
              >
                <option value="left">Izquierda</option>
                <option value="center">Centro</option>
                <option value="right">Derecha</option>
              </select>
            </div>

            <!-- Width control -->
            <div>
              <label class="block text-xs font-medium text-gray-600 mb-2">Ancho</label>
              <select
                v-model="selectedBlock.content.decorationWidth"
                @change="updateBlockContent"
                class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
              >
                <option value="30%">30%</option>
                <option value="40%">40%</option>
                <option value="50%">50%</option>
                <option value="60%">60%</option>
                <option value="70%">70%</option>
                <option value="100%">100%</option>
              </select>
              <p class="text-xs text-gray-500 mt-1">Ancho de la decoración respecto al bloque</p>
            </div>

            <!-- Opacity control -->
            <div>
              <label class="block text-xs font-medium text-gray-600 mb-2">Opacidad</label>
              <div class="flex items-center space-x-2">
                <input
                  type="range"
                  v-model="selectedBlock.content.decorationOpacity"
                  @input="updateBlockContent"
                  min="0.1"
                  max="1"
                  step="0.1"
                  class="flex-1"
                />
                <span class="text-xs text-gray-600 w-10">{{ (selectedBlock.content.decorationOpacity * 100).toFixed(0) }}%</span>
              </div>
            </div>

            <!-- Info box -->
            <div class="bg-blue-50 rounded p-2 text-xs text-blue-700">
              <strong>💡 Consejo:</strong> La decoración aparecerá como fondo del bloque. Ajusta la opacidad para que no interfiera con la legibilidad del contenido.
            </div>
          </div>
        </div>
      </div>

      <!-- Block-specific properties -->
      <div class="pt-4 border-t space-y-3">
        <h3 class="text-sm font-semibold text-gray-700">Contenido</h3>

        <!-- Heading Block -->
        <div v-if="selectedBlock.type === 'heading'">
          <label class="block text-xs font-medium text-gray-600 mb-1">Nivel de título</label>
          <select
            v-model="selectedBlock.content.level"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            @change="updateBlockContent"
          >
            <option value="h1">H1 - Muy grande</option>
            <option value="h2">H2 - Grande</option>
            <option value="h3">H3 - Mediano</option>
            <option value="h4">H4 - Pequeño</option>
          </select>
        </div>

        <!-- Image Block -->
        <div v-if="selectedBlock.type === 'image'">
          <label class="block text-xs font-medium text-gray-600 mb-2">Subir imagen</label>
          <input
            type="file"
            @change="uploadImage"
            accept="image/*"
            class="w-full text-xs text-gray-500 file:mr-2 file:py-2 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploading" class="text-xs text-gray-500 mt-1">Subiendo...</p>

          <label class="block text-xs font-medium text-gray-600 mb-1 mt-3">Ancho (px o %)</label>
          <input
            v-model="selectedBlock.content.width"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="300px, 50%, auto"
            @change="updateBlockContent"
          />

          <label class="block text-xs font-medium text-gray-600 mb-1 mt-3">Alto (px o %)</label>
          <input
            v-model="selectedBlock.content.height"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            placeholder="200px, auto"
            @change="updateBlockContent"
          />

          <label class="block text-xs font-medium text-gray-600 mb-1 mt-3">Texto alternativo</label>
          <input
            v-model="selectedBlock.content.alt"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            @change="updateBlockContent"
          />

          <label class="block text-xs font-medium text-gray-600 mb-1 mt-3">Pie de foto</label>
          <input
            v-model="selectedBlock.content.caption"
            type="text"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            @change="updateBlockContent"
          />
        </div>

        <!-- Carousel Block -->
        <div v-if="selectedBlock.type === 'carousel'">
          <label class="block text-xs font-medium text-gray-600 mb-2">Agregar imágenes</label>
          <input
            type="file"
            @change="uploadCarouselImages"
            accept="image/*"
            multiple
            class="w-full text-xs text-gray-500 file:mr-2 file:py-2 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
          />
          <p v-if="uploading" class="text-xs text-gray-500 mt-1">Subiendo...</p>

          <div class="mt-3">
            <label class="block text-xs font-medium text-gray-600 mb-1">Fotos visibles a la vez</label>
            <input
              v-model.number="selectedBlock.content.slidesPerView"
              type="number"
              min="1"
              max="5"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
            <p class="text-xs text-gray-500 mt-1">Número de fotos que se muestran simultáneamente</p>
          </div>

          <div class="mt-3">
            <label class="flex items-center gap-2">
              <input
                v-model="selectedBlock.content.autoplay"
                type="checkbox"
                @change="updateBlockContent"
              />
              <span class="text-xs">Reproducción automática</span>
            </label>
          </div>

          <div v-if="selectedBlock.content.autoplay" class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Intervalo (ms)</label>
            <input
              v-model.number="selectedBlock.content.interval"
              type="number"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
        </div>

        <!-- Event Info Block -->
        <div v-if="selectedBlock.type === 'eventInfo'" class="space-y-2">
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Fecha</label>
            <input
              v-model="selectedBlock.content.date"
              type="date"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Hora</label>
            <input
              v-model="selectedBlock.content.time"
              type="time"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Lugar</label>
            <input
              v-model="selectedBlock.content.venue"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Dirección</label>
            <input
              v-model="selectedBlock.content.address"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
        </div>

        <!-- Countdown Block -->
        <div v-if="selectedBlock.type === 'countdown'">
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Título</label>
            <input
              v-model="selectedBlock.content.title"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              placeholder="Faltan..."
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Fecha objetivo</label>
            <input
              v-model="selectedBlock.content.targetDate"
              type="datetime-local"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
        </div>

        <!-- Map Block -->
        <div v-if="selectedBlock.type === 'map'">
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Título</label>
            <input
              v-model="selectedBlock.content.title"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Latitud</label>
            <input
              v-model.number="selectedBlock.content.lat"
              type="number"
              step="0.000001"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Longitud</label>
            <input
              v-model.number="selectedBlock.content.lng"
              type="number"
              step="0.000001"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
        </div>

        <!-- Button Block -->
        <div v-if="selectedBlock.type === 'button'">
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Texto del botón</label>
            <input
              v-model="selectedBlock.content.text"
              type="text"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">URL</label>
            <input
              v-model="selectedBlock.content.url"
              type="url"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              placeholder="https://..."
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Estilo</label>
            <select
              v-model="selectedBlock.content.style"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            >
              <option value="primary">Primario</option>
              <option value="outline">Contorno</option>
              <option value="secondary">Secundario</option>
            </select>
          </div>
        </div>

        <!-- Divider Block -->
        <div v-if="selectedBlock.type === 'divider'">
          <div>
            <label class="block text-xs font-medium text-gray-600 mb-1">Color</label>
            <input
              v-model="selectedBlock.content.color"
              type="color"
              class="w-full h-9 rounded cursor-pointer"
              @change="updateBlockContent"
            />
          </div>
          <div class="mt-2">
            <label class="block text-xs font-medium text-gray-600 mb-1">Estilo</label>
            <select
              v-model="selectedBlock.content.style"
              class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
              @change="updateBlockContent"
            >
              <option value="solid">Sólido</option>
              <option value="dashed">Punteado</option>
              <option value="dotted">Puntos</option>
            </select>
          </div>
        </div>

        <!-- Spacer Block -->
        <div v-if="selectedBlock.type === 'spacer'">
          <label class="block text-xs font-medium text-gray-600 mb-1">Altura (px)</label>
          <input
            v-model.number="selectedBlock.content.height"
            type="number"
            class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
            @change="updateBlockContent"
          />
        </div>

        <!-- Form Block -->
        <div v-if="selectedBlock.type === 'form'" class="space-y-3">
          <!-- Basic Info Section (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.basic = !formSectionsCollapsed.basic"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">📝 Información Básica</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.basic ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.basic" class="p-3 space-y-3">
              <div>
                <label class="block text-xs font-medium text-gray-600 mb-1">Título</label>
                <input
                  v-model="selectedBlock.content.title"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
                  @change="updateBlockContent"
                />
              </div>

              <div>
                <label class="block text-xs font-medium text-gray-600 mb-1">Descripción</label>
                <textarea
                  v-model="selectedBlock.content.description"
                  rows="3"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
                  @change="updateBlockContent"
                ></textarea>
              </div>

              <div>
                <label class="block text-xs font-medium text-gray-600 mb-1">Subtítulo</label>
                <input
                  v-model="selectedBlock.content.subtitle"
                  type="text"
                  class="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm"
                  @change="updateBlockContent"
                />
              </div>

              <!-- Estilos del formulario -->
              <div class="pt-3 border-t space-y-3">
                <h4 class="text-xs font-semibold text-gray-600">🎨 Estilos</h4>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Color de texto</label>
                  <input
                    v-model="selectedBlock.style.color"
                    type="color"
                    class="w-full h-9 rounded cursor-pointer"
                    @change="updateBlockStyle"
                  />
                </div>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Color de fondo</label>
                  <input
                    v-model="selectedBlock.style.backgroundColor"
                    type="color"
                    class="w-full h-9 rounded cursor-pointer"
                    @change="updateBlockStyle"
                  />
                </div>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Fuente</label>
                  <select
                    v-model="selectedBlock.style.fontFamily"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockStyle"
                    :style="{ fontFamily: selectedBlock.style.fontFamily }"
                  >
                    <optgroup label="Sans-serif (modernas)">
                      <option value="Arial">Arial</option>
                      <option value="Montserrat">Montserrat</option>
                      <option value="Open Sans">Open Sans</option>
                      <option value="Raleway">Raleway</option>
                      <option value="Poppins">Poppins</option>
                      <option value="Roboto">Roboto</option>
                      <option value="Oswald">Oswald</option>
                      <option value="Bebas Neue">Bebas Neue</option>
                    </optgroup>
                    <optgroup label="Serif (elegantes)">
                      <option value="Playfair Display">Playfair Display</option>
                      <option value="Lora">Lora</option>
                      <option value="Merriweather">Merriweather</option>
                      <option value="Crimson Text">Crimson Text</option>
                    </optgroup>
                    <optgroup label="Script (caligráficas)">
                      <option value="Dancing Script">Dancing Script</option>
                      <option value="Pacifico">Pacifico</option>
                      <option value="Great Vibes">Great Vibes</option>
                      <option value="Satisfy">Satisfy</option>
                    </optgroup>
                    <optgroup label="Monospace">
                      <option value="Courier New">Courier New</option>
                    </optgroup>
                  </select>
                </div>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Tamaño de fuente</label>
                  <input
                    v-model="selectedBlock.style.fontSize"
                    type="text"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    placeholder="16px"
                    @change="updateBlockStyle"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Phone Field Configuration (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.phone = !formSectionsCollapsed.phone"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">📱 Campo de Teléfono</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.phone ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.phone" class="p-3 space-y-2">
              <div>
                <label class="block text-xs text-gray-600 mb-1">Ancho (px o %)</label>
                <input
                  v-model="selectedBlock.content.phoneFieldWidth"
                  type="text"
                  placeholder="100%, 300px, 50%"
                  class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                  @change="updateBlockContent"
                />
              </div>

              <div class="grid grid-cols-2 gap-2">
                <div>
                  <label class="block text-xs text-gray-600 mb-1">Margen superior</label>
                  <input
                    v-model="selectedBlock.content.phoneFieldMarginTop"
                    type="text"
                    placeholder="0px, 16px"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  />
                </div>
                <div>
                  <label class="block text-xs text-gray-600 mb-1">Margen inferior</label>
                  <input
                    v-model="selectedBlock.content.phoneFieldMarginBottom"
                    type="text"
                    placeholder="24px, 32px"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  />
                </div>
              </div>
            </div>
          </div>

          <!-- Greeting Message Configuration (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.greeting = !formSectionsCollapsed.greeting"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">💬 Mensaje de Saludo</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.greeting ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.greeting" class="p-3">
              <p class="text-xs text-gray-500 mb-2">Usa {name} para el nombre y {guests} para los acompañantes</p>

              <textarea
                v-model="selectedBlock.content.greetingMessage"
                rows="3"
                class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                placeholder="¡Hola {name}! Esta invitación es para ti y {guests}"
                @change="updateBlockContent"
              ></textarea>

              <div class="mt-2 p-2 bg-blue-50 rounded text-xs text-blue-700">
                <strong>Ejemplo:</strong> Si el invitado es "Juan Pérez" con acompañantes "María y Pedro", se mostrará: "¡Hola Juan Pérez! Esta invitación es para ti y María y Pedro"
              </div>
            </div>
          </div>

          <!-- Button Configuration (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.button = !formSectionsCollapsed.button"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">🔘 Botón de Envío</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.button ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.button" class="p-3 space-y-2">
              <div>
                <label class="block text-xs text-gray-600 mb-1">Texto del botón</label>
                <input
                  v-model="selectedBlock.content.buttonText"
                  type="text"
                  class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                  @change="updateBlockContent"
                />
              </div>

              <div>
                <label class="block text-xs text-gray-600 mb-1">Color del botón</label>
                <input
                  v-model="selectedBlock.content.buttonColor"
                  type="color"
                  class="w-full h-9 rounded cursor-pointer"
                  @change="updateBlockContent"
                />
              </div>

              <div>
                <label class="block text-xs text-gray-600 mb-1">Color del texto</label>
                <input
                  v-model="selectedBlock.content.buttonTextColor"
                  type="color"
                  class="w-full h-9 rounded cursor-pointer"
                  @change="updateBlockContent"
                />
              </div>

              <div>
                <label class="block text-xs text-gray-600 mb-1">Tamaño de fuente</label>
                <input
                  v-model="selectedBlock.content.buttonFontSize"
                  type="text"
                  placeholder="1rem, 18px"
                  class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                  @change="updateBlockContent"
                />
              </div>
            </div>
          </div>

          <!-- Custom Fields Section (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.fields = !formSectionsCollapsed.fields"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">✏️ Campos Personalizados</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.fields ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.fields" class="p-3">
              <div class="flex items-center justify-between mb-3">
                <span class="text-xs text-gray-600">{{ selectedBlock.content.customFields?.length || 0 }} campo(s)</span>
                <button
                  @click="addFormField"
                  class="px-2 py-1 bg-purple-600 text-white rounded text-xs hover:bg-purple-700"
                >
                  + Agregar campo
                </button>
              </div>

            <div v-for="(field, index) in selectedBlock.content.customFields" :key="index" class="mb-4 p-3 border border-gray-200 rounded-lg">
              <div class="flex items-center justify-between mb-2">
                <span class="text-xs font-semibold text-gray-700">Campo {{ index + 1 }}</span>
                <button
                  @click="removeFormField(index)"
                  class="text-red-600 hover:text-red-800 text-xs"
                >
                  🗑️ Eliminar
                </button>
              </div>

              <div class="space-y-2">
                <div>
                  <label class="block text-xs text-gray-600 mb-1">Etiqueta</label>
                  <input
                    v-model="field.label"
                    type="text"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  />
                </div>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Tipo</label>
                  <select
                    v-model="field.type"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  >
                    <option value="text">Texto</option>
                    <option value="select">Lista</option>
                    <option value="radio">Radio Buttons</option>
                  </select>
                </div>

                <div v-if="field.type !== 'radio'">
                  <label class="block text-xs text-gray-600 mb-1">Placeholder</label>
                  <input
                    v-model="field.placeholder"
                    type="text"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  />
                </div>

                <div>
                  <label class="flex items-center space-x-2">
                    <input
                      v-model="field.required"
                      type="checkbox"
                      @change="updateBlockContent"
                    />
                    <span class="text-xs">Campo requerido</span>
                  </label>
                </div>

                <div>
                  <label class="block text-xs text-gray-600 mb-1">Ancho (px o %)</label>
                  <input
                    v-model="field.width"
                    type="text"
                    placeholder="100%, 300px, 50%"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                    @change="updateBlockContent"
                  />
                </div>

                <div class="grid grid-cols-2 gap-2">
                  <div>
                    <label class="block text-xs text-gray-600 mb-1">Margen superior</label>
                    <input
                      v-model="field.marginTop"
                      type="text"
                      placeholder="0px, 16px"
                      class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                      @change="updateBlockContent"
                    />
                  </div>
                  <div>
                    <label class="block text-xs text-gray-600 mb-1">Margen inferior</label>
                    <input
                      v-model="field.marginBottom"
                      type="text"
                      placeholder="16px, 24px"
                      class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                      @change="updateBlockContent"
                    />
                  </div>
                </div>

                <div v-if="field.type === 'select' || field.type === 'radio'">
                  <label class="block text-xs text-gray-600 mb-1">
                    {{ field.type === 'radio' ? 'Opciones de radio (una por línea)' : 'Opciones de lista (una por línea)' }}
                  </label>
                  <textarea
                    :value="field.options ? field.options.join('\n') : ''"
                    @input="updateFieldOptions(index, $event.target.value)"
                    @keydown.stop
                    rows="4"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs resize-y"
                    placeholder="Escribe cada opción en una línea nueva&#10;Opción 1&#10;Opción 2&#10;Opción 3"
                  ></textarea>
                </div>

                <!-- Visibility Rules Section -->
                <div class="mt-3 pt-3 border-t border-gray-200">
                  <div class="mb-2">
                    <label class="flex items-center space-x-2 cursor-pointer">
                      <input
                        v-model="field.visibilityRules.enabled"
                        type="checkbox"
                        @change="updateBlockContent"
                        class="w-3 h-3"
                      />
                      <span class="text-xs font-medium text-gray-700">👁️ Reglas de visibilidad</span>
                    </label>
                    <p class="text-xs text-gray-500 mt-1 ml-5">Mostrar este campo solo si se cumple una condición</p>
                  </div>

                  <div v-if="field.visibilityRules.enabled" class="ml-5 space-y-2 mt-2 p-2 bg-blue-50 rounded">
                    <div>
                      <label class="block text-xs text-gray-600 mb-1">Condición basada en</label>
                      <select
                        v-model="field.visibilityRules.condition"
                        class="w-full px-2 py-1 border border-gray-300 rounded text-xs bg-white"
                        @change="updateBlockContent"
                      >
                        <option value="">Seleccionar condición...</option>
                        <option value="accommodationAllowed">Hospedaje permitido</option>
                        <option value="eventHasAccommodation">Evento con hospedaje</option>
                        <option value="numberOfGuests">Número de invitados</option>
                        <option value="hasGuestNames">Tiene nombres de acompañantes</option>
                      </select>
                    </div>

                    <div v-if="field.visibilityRules.condition">
                      <label class="block text-xs text-gray-600 mb-1">Mostrar cuando</label>
                      <select
                        v-model="field.visibilityRules.showWhen"
                        class="w-full px-2 py-1 border border-gray-300 rounded text-xs bg-white"
                        @change="updateBlockContent"
                      >
                        <option value="equals">Es igual a</option>
                        <option value="notEquals">No es igual a</option>
                        <option value="greaterThan">Mayor que</option>
                        <option value="lessThan">Menor que</option>
                      </select>
                    </div>

                    <div v-if="field.visibilityRules.condition && field.visibilityRules.showWhen">
                      <label class="block text-xs text-gray-600 mb-1">Valor</label>
                      <select
                        v-if="field.visibilityRules.condition === 'accommodationAllowed' || field.visibilityRules.condition === 'eventHasAccommodation'"
                        v-model="field.visibilityRules.value"
                        class="w-full px-2 py-1 border border-gray-300 rounded text-xs bg-white"
                        @change="updateBlockContent"
                      >
                        <option value="true">Sí</option>
                        <option value="false">No</option>
                      </select>
                      <input
                        v-else-if="field.visibilityRules.condition === 'numberOfGuests'"
                        v-model="field.visibilityRules.value"
                        type="number"
                        min="0"
                        class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                        @change="updateBlockContent"
                        placeholder="Ej: 2"
                      />
                      <select
                        v-else-if="field.visibilityRules.condition === 'hasGuestNames'"
                        v-model="field.visibilityRules.value"
                        class="w-full px-2 py-1 border border-gray-300 rounded text-xs bg-white"
                        @change="updateBlockContent"
                      >
                        <option value="true">Tiene nombres</option>
                        <option value="false">No tiene nombres</option>
                      </select>
                    </div>

                    <div class="text-xs text-blue-700 mt-2 p-2 bg-blue-100 rounded">
                      <strong>Ejemplo:</strong>
                      <span v-if="field.visibilityRules.condition === 'accommodationAllowed'">
                        Este campo se mostrará solo a invitados que {{ field.visibilityRules.value === 'true' ? 'tienen' : 'NO tienen' }} permitido el hospedaje.
                      </span>
                      <span v-else-if="field.visibilityRules.condition === 'eventHasAccommodation'">
                        Este campo se mostrará solo si el evento {{ field.visibilityRules.value === 'true' ? 'incluye' : 'NO incluye' }} hospedaje.
                      </span>
                      <span v-else-if="field.visibilityRules.condition === 'numberOfGuests'">
                        Este campo se mostrará solo si el número de invitados es {{ field.visibilityRules.showWhen === 'greaterThan' ? 'mayor que' : field.visibilityRules.showWhen === 'lessThan' ? 'menor que' : field.visibilityRules.showWhen === 'equals' ? 'igual a' : 'diferente de' }} {{ field.visibilityRules.value }}.
                      </span>
                      <span v-else-if="field.visibilityRules.condition === 'hasGuestNames'">
                        Este campo se mostrará solo si el invitado {{ field.visibilityRules.value === 'true' ? 'tiene' : 'NO tiene' }} nombres de acompañantes registrados.
                      </span>
                      <span v-else>
                        Configura la condición para ver el ejemplo.
                      </span>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <!-- Decoration Section (Collapsible) -->
          <div class="border rounded-lg">
            <button
              @click="formSectionsCollapsed.decoration = !formSectionsCollapsed.decoration"
              class="w-full px-3 py-2 flex items-center justify-between bg-gray-50 hover:bg-gray-100 rounded-t-lg"
            >
              <span class="text-xs font-semibold text-gray-700">🎨 Decoración de Fondo</span>
              <span class="text-gray-500">{{ formSectionsCollapsed.decoration ? '▼' : '▲' }}</span>
            </button>
            <div v-show="!formSectionsCollapsed.decoration" class="p-3 space-y-3">
              <!-- Enable decoration toggle -->
              <div class="flex items-center justify-between">
                <label class="text-xs text-gray-700 font-medium">Activar decoración</label>
                <input
                  type="checkbox"
                  v-model="selectedBlock.content.decorationEnabled"
                  @change="updateBlockContent"
                  class="w-4 h-4 text-purple-600 rounded focus:ring-purple-500"
                />
              </div>

              <div v-if="selectedBlock.content.decorationEnabled" class="space-y-3 pt-2 border-t">
                <!-- Upload decoration image -->
                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-2">Imagen de decoración</label>
                  <input
                    type="file"
                    @change="uploadFormDecoration"
                    accept="image/*"
                    class="w-full text-xs text-gray-500 file:mr-2 file:py-1.5 file:px-3 file:rounded file:border-0 file:text-xs file:font-semibold file:bg-purple-50 file:text-purple-700 hover:file:bg-purple-100"
                  />
                  <p v-if="uploading" class="text-xs text-blue-600 mt-1">Subiendo imagen...</p>

                  <!-- Preview of current decoration -->
                  <div v-if="selectedBlock.content.decorationImage" class="mt-2">
                    <img
                      :src="selectedBlock.content.decorationImage"
                      alt="Decoración"
                      class="w-full h-24 object-contain rounded border border-gray-200"
                    />
                    <button
                      @click="selectedBlock.content.decorationImage = ''; updateBlockContent()"
                      class="mt-1 text-xs text-red-600 hover:text-red-800"
                    >
                      Eliminar imagen
                    </button>
                  </div>
                </div>

                <!-- Position selection -->
                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-2">Posición</label>
                  <select
                    v-model="selectedBlock.content.decorationPosition"
                    @change="updateBlockContent"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                  >
                    <option value="left">Izquierda</option>
                    <option value="center">Centro</option>
                    <option value="right">Derecha</option>
                  </select>
                </div>

                <!-- Width control -->
                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-2">Ancho</label>
                  <select
                    v-model="selectedBlock.content.decorationWidth"
                    @change="updateBlockContent"
                    class="w-full px-2 py-1 border border-gray-300 rounded text-xs"
                  >
                    <option value="30%">30%</option>
                    <option value="40%">40%</option>
                    <option value="50%">50%</option>
                    <option value="60%">60%</option>
                    <option value="70%">70%</option>
                    <option value="100%">100%</option>
                  </select>
                  <p class="text-xs text-gray-500 mt-1">Ancho de la decoración respecto al formulario</p>
                </div>

                <!-- Opacity control -->
                <div>
                  <label class="block text-xs font-medium text-gray-600 mb-2">Opacidad</label>
                  <div class="flex items-center space-x-2">
                    <input
                      type="range"
                      v-model="selectedBlock.content.decorationOpacity"
                      @input="updateBlockContent"
                      min="0.1"
                      max="1"
                      step="0.1"
                      class="flex-1"
                    />
                    <span class="text-xs text-gray-600 w-10">{{ (selectedBlock.content.decorationOpacity * 100).toFixed(0) }}%</span>
                  </div>
                </div>

                <!-- Info box -->
                <div class="bg-blue-50 rounded p-2 text-xs text-blue-700">
                  <strong>💡 Consejo:</strong> La decoración aparecerá como fondo del formulario completo. Ajusta la opacidad para que no interfiera con la legibilidad del texto.
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue'
import { useInvitationStore } from '../../stores/invitationStore'
import { invitationService } from '../../services/api'

const store = useInvitationStore()
const uploading = ref(false)

// Collapsible sections state for form properties
const formSectionsCollapsed = ref({
  basic: false,
  phone: true,
  greeting: true,
  button: true,
  fields: false,
  decoration: true
})

const selectedBlock = computed(() => {
  if (!store.selectedBlockId) return null
  return store.invitation.blocks.find(b => b.id === store.selectedBlockId)
})

const getBlockLabel = (type) => {
  const labels = {
    text: 'Texto',
    heading: 'Título',
    image: 'Imagen',
    carousel: 'Carrusel',
    eventInfo: 'Info del Evento',
    countdown: 'Contador',
    map: 'Mapa',
    button: 'Botón',
    divider: 'Separador',
    spacer: 'Espaciador',
    form: 'Formulario'
  }
  return labels[type] || type
}

const updateBlockStyle = () => {
  store.updateBlockStyle(store.selectedBlockId, selectedBlock.value.style)
}

const updateBlockContent = () => {
  store.updateBlockContent(store.selectedBlockId, selectedBlock.value.content)
}

const uploadImage = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploading.value = true
  const formData = new FormData()
  formData.append('file', file)

  try {
    const response = await invitationService.uploadImage(formData)
    // Store only the relative URL returned by the backend
    selectedBlock.value.content.url = response.data.url
    updateBlockContent()
  } catch (error) {
    console.error('Error uploading image:', error)
    alert('Error al subir la imagen')
  } finally {
    uploading.value = false
  }
}

const uploadCarouselImages = async (event) => {
  const files = Array.from(event.target.files)
  if (files.length === 0) return

  uploading.value = true

  try {
    const uploadPromises = files.map(file => {
      const formData = new FormData()
      formData.append('file', file)
      return invitationService.uploadImage(formData)
    })

    const responses = await Promise.all(uploadPromises)
    const imageUrls = responses.map(r => r.data.url)

    if (!selectedBlock.value.content.images) {
      selectedBlock.value.content.images = []
    }
    selectedBlock.value.content.images.push(...imageUrls)
    updateBlockContent()
  } catch (error) {
    console.error('Error uploading images:', error)
    alert('Error al subir las imágenes')
  } finally {
    uploading.value = false
  }
}

// Form field management functions
const addFormField = () => {
  if (!selectedBlock.value.content.customFields) {
    selectedBlock.value.content.customFields = []
  }
  selectedBlock.value.content.customFields.push({
    label: 'Nuevo campo',
    type: 'text',
    placeholder: 'Escribir aquí...',
    required: false,
    options: [],
    width: '100%',
    marginTop: '0px',
    marginBottom: '16px',
    visibilityRules: {
      enabled: false,
      condition: '',
      showWhen: 'equals',
      value: ''
    }
  })
  updateBlockContent()
}

const removeFormField = (index) => {
  selectedBlock.value.content.customFields.splice(index, 1)
  updateBlockContent()
}

const updateFieldOptions = (index, value) => {
  const options = value.split('\n').filter(opt => opt.trim() !== '')
  selectedBlock.value.content.customFields[index].options = options
  updateBlockContent()
}

// Upload decoration image for form
const uploadFormDecoration = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await invitationService.uploadImage(formData)
    // Construir la URL completa con el host del backend
    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'
    const baseUrl = apiUrl.replace('/api', '')
    const fullUrl = baseUrl + response.data.url

    console.log('Decoration image uploaded:', {
      relativeUrl: response.data.url,
      baseUrl: baseUrl,
      fullUrl: fullUrl
    })

    selectedBlock.value.content.decorationImage = fullUrl
    updateBlockContent()
  } catch (error) {
    console.error('Error uploading decoration:', error)
    alert('Error al subir la imagen de decoración')
  } finally {
    uploading.value = false
    event.target.value = ''
  }
}

// Upload decoration image for any block (heading, text, carousel, map, etc.)
const uploadBlockDecoration = async (event) => {
  const file = event.target.files[0]
  if (!file) return

  uploading.value = true
  try {
    const formData = new FormData()
    formData.append('file', file)

    const response = await invitationService.uploadImage(formData)
    // Construir la URL completa con el host del backend
    const apiUrl = import.meta.env.VITE_API_URL || 'http://localhost:5000/api'
    const baseUrl = apiUrl.replace('/api', '')
    const fullUrl = baseUrl + response.data.url

    console.log('Block decoration image uploaded:', {
      blockType: selectedBlock.value.type,
      relativeUrl: response.data.url,
      baseUrl: baseUrl,
      fullUrl: fullUrl
    })

    selectedBlock.value.content.decorationImage = fullUrl
    updateBlockContent()
  } catch (error) {
    console.error('Error uploading block decoration:', error)
    alert('Error al subir la imagen de decoración')
  } finally {
    uploading.value = false
    event.target.value = ''
  }
}
</script>
